using SocketIOClient;
using System.Text.Json;
using SocketIONamespace = SocketIOClient.SocketIO;
using APIBanca.Repositories;

namespace APIBanca.Handlers
{

    public class BankSocketHandler : IDisposable
    {
        private readonly SocketIONamespace _socket;
        private readonly ILogger<BankSocketHandler> _logger;
        private readonly IServiceScopeFactory _scopeFactory; // servicios scoped desde singleton

        // Credenciales necesarias para ingresar al banco central
        private readonly string _url = "http://137.184.36.3:6000";
        private readonly string _bankId = "B01";
        private readonly string _bankName = "Banca Prometedora";
        private readonly string _token = "BANK-CENTRAL-IC8057-2025";

        public BankSocketHandler(ILogger<BankSocketHandler> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            _logger.LogInformation("inicio BankSocketHandler");
            _logger.LogInformation($"URL: {_url}");
            _logger.LogInformation($"Bank ID: {_bankId}");

            // Configuración de reconexión esto porque al inicio no funcionaba correctamente 
            var options = new SocketIOOptions
            {
                Reconnection = true,                  // Reconecta automaticamente 
                ReconnectionDelay = 5000,             // Espera 5 segundos por intento 
                ReconnectionAttempts = 10,            // MSolo 10 intentos 
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
                ConnectionTimeout = TimeSpan.FromSeconds(20)
            };

            // Parametros a autenticar según lo solicitado por el profe
            options.Auth = new Dictionary<string, string>
            {
                { "bankId", _bankId },
                { "bankName", _bankName },
                { "token", _token }
            };

            _socket = new SocketIONamespace(_url, options);

            // Creamos los handlers para los eventos
            ConfigureEvents();

            _socket.OnConnected += (sender, e) =>
            {
                Console.WriteLine("═════════════════════════════════════════");
                Console.WriteLine("SIU NOS CONECTAMOS");
                Console.WriteLine($"Socket ID: {_socket.Id}");
                Console.WriteLine("═════════════════════════════════════════");
                _logger.LogInformation($"conectado ID: {_socket.Id}");
            };

            _socket.OnDisconnected += (sender, e) =>
            {
                Console.WriteLine($"DESCONECTADO: {e}");
                _logger.LogWarning($"Desconectado: {e}");
            };

            _socket.OnError += (sender, e) =>
            {
                Console.WriteLine($"ERROR: {e}");
                _logger.LogError($"Error: {e}");
            };

            _logger.LogInformation("BankSocketHandler configurado");
        }


        
        //Configuración para escucha 
        private void ConfigureEvents()
        {
            // Para informa sobre al autenticación 
            _socket.On("authenticated", response =>
            {
                _logger.LogInformation("AUTENTICADO!");
            });

            // Si el banco nos acepta la transferencia
            _socket.On("transfer.accept", response =>
            {
                _logger.LogInformation("transfer.accept");
            });

            // Listener principal: procesa array de eventos del Banco Central
            // Agarra  el array que envia el profe para los eventos
            _socket.On("event", async response =>
            {
                try
                {
                    // Aqui obtenemos los datos del evento
                    var evt = response.GetValue<JsonElement>(0);

                    var eventType = evt.GetProperty("type").GetString();
                    var eventData = evt.GetProperty("data");

                    Console.WriteLine($"═══ Procesando evento: {eventType} ═══");

                    // Enviamos el evento dependiendo del tipo que llegue
                    switch (eventType)
                    {
                        case "transfer.reserve":
                            await HandleTransferReserve(eventData);
                            break;

                        case "transfer.credit":
                            await HandleTransferCredit(eventData);
                            break;

                        case "transfer.debit":
                            await HandleTransferDebit(eventData);
                            break;

                        case "transfer.commit":
                            await HandleTransferCommit(eventData);
                            break;

                        case "transfer.rollback":
                            await HandleTransferRollback(eventData);
                            break;
                    }
                }
                // Por si da algún error fuera del flujo
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando evento");
                }
            });
        }


        public async Task ConnectAsync()
        {
            try
            {
                _logger.LogInformation("Conectando...");

                await _socket.ConnectAsync();
                await Task.Delay(2000); // esperamos un poco para ver si nos conectamos

                if (_socket.Connected)
                    _logger.LogInformation($"CONECTADO ID: {_socket.Id}");
                else
                    _logger.LogWarning("No conectado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción conectando");
            }
        }


        
        // iniciamos la transferencia enviando el intent
        public async Task SendInterbankTransfer(object data)
        {
            if (!_socket.Connected)
                throw new InvalidOperationException("No conectado");

            _logger.LogInformation("Enviando transfer.intent");
            _logger.LogInformation($"Data: {JsonSerializer.Serialize(data)}");

            await _socket.EmitAsync("event", data);

            _logger.LogInformation("Emit completado");
        }


        // Aqui congelamos los montos y vemos si hay fondos 
        private async Task HandleTransferReserve(JsonElement data)
        {
            Console.WriteLine("═══ transfer.reserve RECIBIDO ═══");

            try
            {
                var txId = data.GetProperty("id").GetString();

                // Scope para usar el singleton  
                using var scope = _scopeFactory.CreateScope();
                var reserveRepo = scope.ServiceProvider.GetRequiredService<Repositories.TransferReserveRepository>();

                // Obtengo mi ultimo mov
                var (fromIban, amount) = await reserveRepo.GetLastMovementAsync();

                if (string.IsNullOrEmpty(fromIban))
                {
                    _logger.LogError("No se pudo obtener el último movimiento");
                    return;
                }

                _logger.LogInformation($"Reservando: {fromIban} → {amount}");

                // congelamos plata
                var (ok, reason) = await reserveRepo.ReserveAsync(fromIban, amount, txId);

                // Respondemos al banco
                await _socket.EmitAsync("event", new
                {
                    type = "transfer.reserve.result",
                    data = new { id = txId, ok, reason }
                });

                _logger.LogInformation("transfer.reserve.result enviado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando transfer.reserve");
            }
        }

        private async Task HandleTransferCredit(JsonElement data)
        {
            Console.WriteLine("═══ transfer.credit RECIBIDO ═══");

            try
            {
                var txId = Guid.Parse(data.GetProperty("id").GetString());
                var to = data.GetProperty("to").GetString();       // IBAN destino
                var from = data.GetProperty("from").GetString();   // IBAN origen 
                var amount = data.GetProperty("amount").GetDecimal();
                var currency = data.GetProperty("currency").GetString();

                using var scope = _scopeFactory.CreateScope();
                var creditRepo = scope.ServiceProvider.GetRequiredService<Repositories.TransferCreditRepository>();

                // Crear movimiento y sumar al saldo
                var (ok, reason) = await creditRepo.CreditAsync(
                    txId, to, from, amount, currency, "Transferencia interbancaria recibida"
                );

                await _socket.EmitAsync("event", new
                {
                    type = "transfer.credit.result",
                    data = new { id = txId.ToString(), ok, reason }
                });

                _logger.LogInformation("transfer.credit.result enviado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando transfer.credit");
            }
        }


        private async Task HandleTransferDebit(JsonElement data)
        {
            Console.WriteLine("═══ transfer.debit RECIBIDO ═══");
            Console.WriteLine(data.ToString());

            var txId = data.GetProperty("id").GetString();
            var fromIban = data.GetProperty("from").GetString(); 

            using var scope = _scopeFactory.CreateScope();
            var debitRepo = scope.ServiceProvider.GetRequiredService<TransferDebitRepository>();

            (bool ok, string? reason) result = await debitRepo.DebitAsync(fromIban);

            await _socket.EmitAsync("event", new
            {
                type = "transfer.debit.result",
                data = new
                {
                    id = txId,
                    ok = result.ok,
                    reason = result.reason
                }
            });

            _logger.LogInformation("transfer.debit.result enviado");
        }



        private async Task HandleTransferCommit(JsonElement data)
        {
            Console.WriteLine("═══ transfer.commit RECIBIDO ═══");
            Console.WriteLine(data.ToString());  

            _logger.LogInformation("transfer.commit DATA: " + data.ToString());



            await Task.CompletedTask;
        }

        private async Task HandleTransferRollback(JsonElement data)
        {
            _logger.LogInformation("transfer.rollback - TODO");
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            try
            {
                _logger.LogInformation("Cerrando conexión con el Banco Central...");

                _socket?.DisconnectAsync().Wait();
                _socket?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar la conexión del socket");
            }
        }
    }
}
