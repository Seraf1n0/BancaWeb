// Archiv con definicion de tipos o modelos de datos necesarios para diferentes cosas

export interface Account {
    account_id: string; // Identificador único en formato IBAN: CR01-{4 banco}-{4 sucursal}-{12 cuenta}
    alias: string;
    tipo: 'Ahorro' | 'Corriente';
    moneda: 'CRC' | 'USD';
    saldo: number;
    propietario: string; // un id de cliente
}

export interface Movement {
    id: string;
    account_id: string; // Cuenta.cuenta_id
    fecha: string; // ISO 8601 (YYYY-MM-DDTHH:mm:ssZ)
    tipo: 'CREDITO' | 'DEBITO';
    descripcion: string;
    moneda: 'CRC' | 'USD';
    saldo: number;
}