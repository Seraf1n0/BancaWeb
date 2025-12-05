using SocketIOClient;
using System.Text.Json;
using SocketIONamespace = SocketIOClient.SocketIO;

namespace APIBanca.Handlers
{
    public class BankSocketHandler : IDisposable
    {
        private readonly SocketIONamespace _socket;
        private readonly ILogger<BankSocketHandler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        
        private readonly string _url = "http://137.184.36.3:6000";
        private readonly string _bankId = "B01";
        private readonly string _bankName = "Banca Prometedora";
        private readonly string _token = "BANK-CENTRAL-IC8057-2025";

        public BankSocketHandler(ILogger<BankSocketHandler> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            
            _logger.LogInformation("🔧 Inicializando BankSocketHandler");
            _logger.LogInformation($"🔧 URL: {_url}");
            _logger.LogInformation($"🔧 Bank ID: {_bankId}");
            
            var options = new SocketIOOptions
            {
                Reconnection = true,
                ReconnectionDelay = 5000,
                ReconnectionAttempts = 10,
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
                ConnectionTimeout = TimeSpan.FromSeconds(20)
            };

            options.Auth = new Dictionary<string, string>
            {
                { "bankId", _bankId },
                { "bankName", _bankName },
                { "token", _token }
            };

            _socket = new SocketIONamespace(_url, options);

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
                Console.WriteLine("═════════════════════════════════════════");
                Console.WriteLine($"DESCONECTADO: {e}");
                Console.WriteLine("═════════════════════════════════════════");
                _logger.LogWarning($"Desconectado: {e}");
            };

            _socket.OnError += (sender, e) =>
            {
                Console.WriteLine("═════════════════════════════════════════");
                Console.WriteLine($"ERROR: {e}");
                Console.WriteLine("═════════════════════════════════════════");
                _logger.LogError($"Error: {e}");
            };


            _socket.OnReconnectAttempt += (sender, attempt) =>
            {
                _logger.LogWarning($"Intento #{attempt}");
            };

            _socket.OnReconnected += (sender, attempt) =>
            {
                _logger.LogInformation($"Reconectado");
            };

            _socket.OnReconnectError += (sender, e) =>
            {
                _logger.LogError($"Error reconexión: {e}");
            };

            _socket.OnReconnectFailed += (sender, e) =>
            {
                _logger.LogError("Reconexión fallida");
            };

            /
            _socket.OnAny((eventName, response) =>
            {
                Console.WriteLine("═══════════════════════════════════");
                Console.WriteLine($"EVENTO RECIBIDO: {eventName}");
                Console.WriteLine($"Data: {response.ToString()}");
                Console.WriteLine("═══════════════════════════════════");
                
                _logger.LogWarning($"EVENTO: {eventName}");
                _logger.LogWarning($"Data: {response.ToString()}");
            });
            
            _logger.LogInformation("🔧 BankSocketHandler configurado");
        }

        private void ConfigureEvents()
        {
            _socket.On("authenticated", response =>
            {
                _logger.LogInformation("AUTENTICADO!");
            });

            _socket.On("transfer.accept", response =>
            {
                Console.WriteLine("═════════════════════════════════════════");
                Console.WriteLine("transfer.accept RECIBIDO");
                Console.WriteLine($"ata: {response.ToString()}");
                Console.WriteLine("═════════════════════════════════════════");
                
                _logger.LogInformation("transfer.accept");
                _logger.LogInformation($"Data: {response.ToString()}");
                
            });

            _socket.On("transfer.reserve", async response =>
            {
                Console.WriteLine("═════════════════════════════════════════");
                Console.WriteLine("transfer.reserve RECIBIDO");
                Console.WriteLine($"Data: {response.ToString()}");
                Console.WriteLine("═════════════════════════════════════════");
                
                _logger.LogInformation("transfer.reserve");
                _logger.LogInformation($"Data: {response.ToString()}");
                
                try
                {
                    var data = response.GetValue<JsonElement>();
                    var txId = data.GetProperty("id").GetString();
                    var from = data.GetProperty("from").GetString();
                    var amount = data.GetProperty("amount").GetDecimal();
                    
                    _logger.LogInformation($"Reservando: {from}, {amount}, {txId}");
                    
                    using var scope = _scopeFactory.CreateScope();
                    var reserveRepo = scope.ServiceProvider.GetRequiredService<Repositories.TransferReserveRepository>();
                    var (ok, reason) = await reserveRepo.ReserveAsync(from, amount, txId);
                    
                    await _socket.EmitAsync("transfer.reserve.result", new
                    {
                        type = "transfer.reserve.result",
                        data = new { id = txId, ok = ok, reason = reason }
                    });
                    
                    if (ok)
                        _logger.LogInformation($" transfer.reserve.result: OK");
                    else
                        _logger.LogWarning($" transfer.reserve.result: FAIL - {reason}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, " Error procesando transfer.reserve");
                }
            });

            _socket.On("transfer.credit", response =>
            {
                _logger.LogInformation("transfer.credit");
                _logger.LogInformation($"Data: {response.ToString()}");
            });

            _socket.On("transfer.debit", response =>
            {
                _logger.LogInformation("transfer.debit");
                _logger.LogInformation($"Data: {response.ToString()}");
            });

            _socket.On("transfer.commit", response =>
            {
                _logger.LogInformation("transfer.commit");
                _logger.LogInformation($"Data: {response.ToString()}");
            });

            _socket.On("transfer.reject", response =>
            {
                _logger.LogInformation("transfer.reject");
                _logger.LogInformation($"Data: {response.ToString()}");
            });

            _socket.On("transfer.rollback", response =>
            {
                _logger.LogInformation("transfer.rollback");
                _logger.LogInformation($"Data: {response.ToString()}");
            });

            _socket.On("transfer.init", response =>
            {
                _logger.LogInformation("transfer.init");
                _logger.LogInformation($"Data: {response.ToString()}");
            });
        }

        public async Task ConnectAsync()
        {
            try
            {
                _logger.LogInformation("Conectando...");
                
                await _socket.ConnectAsync();
                
                await Task.Delay(2000);
                
                if (_socket.Connected)
                {
                    _logger.LogInformation($"ONECTADO ID: {_socket.Id}");
                }
                else
                {
                    _logger.LogWarning("No conectado");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Excepción");
            }
        }

        public async Task SendInterbankTransfer(object data)
        {
            if (!_socket.Connected)
            {
                throw new InvalidOperationException("No conectado");
            }

            _logger.LogInformation("Enviando transfer.intent");
            _logger.LogInformation($"Data: {JsonSerializer.Serialize(data)}");
            
            await _socket.EmitAsync("transfer.intent", data);
            
            _logger.LogInformation("Emit completado, esperando...");
        }

        public void Dispose()
        {
            _logger.LogInformation("errando conexión :c");
            _socket?.DisconnectAsync().Wait();
            _socket?.Dispose();
        }
    }
}