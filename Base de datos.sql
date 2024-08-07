use master

create database PruebaBancoA_MiguelGonzalez

use PruebaBancoA_MiguelGonzalez

create table Tarjetas(
	Id INT PRIMARY KEY IDENTITY(1,1) not null,
    Trajeta VARCHAR(20) NOT NULL,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
	LimiteCredito DECIMAL(10, 2) NOT NULL
)


CREATE TABLE Compras (
    Id INT PRIMARY KEY  not null,
    TarjetaId INT NOT NULL,
    FechaCompra DATETIME NOT NULL,
    Monto DECIMAL(10, 2) NOT NULL,
    Descripcion NVARCHAR(255),
    CONSTRAINT FK_TarjetaCompras FOREIGN KEY (TarjetaId) REFERENCES Tarjetas(Id)
);


CREATE TABLE Pagos (
    Id INT PRIMARY KEY not null,
    TarjetaId INT NOT NULL,
    FechaCompra DATETIME NOT NULL,
    Monto DECIMAL(10, 2) NOT NULL,
    Descripcion NVARCHAR(255),
    CONSTRAINT FK_TarjetaPago FOREIGN KEY (TarjetaId) REFERENCES Tarjetas(Id)
);




INSERT INTO Tarjetas (Trajeta, Nombre, Apellido, LimiteCredito) VALUES 
('5454 1234 9023 2345', 'David', 'Castro', 2000),
('5656 2345 7689 6543', 'Jose', 'Martinez', 3000),
('5454 9846 0567 2345', 'Juan', 'Perez', 1000)




select * from Tarjetas

select * from Compras

select * from Pagos
