-- Creación de la base de datos
CREATE DATABASE ProyectoPokedex;
GO

-- Selección de la base de datos
USE ProyectoPokedex;
GO

-- Creación de la tabla Roles
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(50) NOT NULL
);

-- Creación de la tabla Usuarios
CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CuentaUsuario VARCHAR(50) NOT NULL UNIQUE,
    ContraseñaHash VARCHAR(255) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    RolId INT NOT NULL,
    FOREIGN KEY (RolId) REFERENCES Roles(Id)
);

-- Creación de la tabla Pokemones
CREATE TABLE Pokemones (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(100) NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    Debilidad VARCHAR(50) NOT NULL,
	HP INT NOT NULL,
	Ataque INT NOT NULL,
	Defensa INT NOT NULL,
);

CREATE TABLE Pokedex (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UsuarioId INT NOT NULL,
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);

CREATE TABLE Pokedex_Pokemon (
    PokedexId INT NOT NULL,
    PokemonId INT NOT NULL,
	Vida INT,
    PRIMARY KEY (PokedexId, PokemonId),
    FOREIGN KEY (PokedexId) REFERENCES Pokedex(Id),
    FOREIGN KEY (PokemonId) REFERENCES Pokemones(Id)
);

-- Creación de la tabla Mensajes
CREATE TABLE Mensajes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RemitenteId INT NOT NULL,
    DestinatarioId INT NOT NULL,
    Contenido TEXT NOT NULL,
    FechaEnvio DATETIME DEFAULT GETDATE(),
    Estado VARCHAR(20) NOT NULL CHECK (Estado IN ('Enviado', 'Leído')),
    FOREIGN KEY (RemitenteId) REFERENCES Usuarios(Id),
    FOREIGN KEY (DestinatarioId) REFERENCES Usuarios(Id)
);

-- Creación de la tabla Retos
CREATE TABLE Retos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RetadorId INT NOT NULL,
    RetadoId INT NOT NULL,
    Estado VARCHAR(20) NOT NULL CHECK (Estado IN ('Pendiente', 'Aceptado', 'Rechazado')),
    FechaReto DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RetadorId) REFERENCES Usuarios(Id),
    FOREIGN KEY (RetadoId) REFERENCES Usuarios(Id)
);

CREATE TABLE Batalla (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Usuario1Id INT NOT NULL,
    Usuario2Id INT NOT NULL,
    PokedexIdUsuario1 INT NOT NULL,
    PokedexIdUsuario2 INT NOT NULL,
    PokemonUsuario1Id INT NOT NULL,
    PokemonUsuario2Id INT NOT NULL,
    GanadorId INT NOT NULL, 
    Fecha DATETIME DEFAULT GETDATE(),
    
    FOREIGN KEY (Usuario1Id) REFERENCES Usuarios(Id),
    FOREIGN KEY (Usuario2Id) REFERENCES Usuarios(Id),

    FOREIGN KEY (PokedexIdUsuario1) REFERENCES Pokedex(Id),
    FOREIGN KEY (PokedexIdUsuario2) REFERENCES Pokedex(Id),

    FOREIGN KEY (PokedexIdUsuario1, PokemonUsuario1Id) REFERENCES Pokedex_Pokemon(PokedexId, PokemonId),
    FOREIGN KEY (PokedexIdUsuario2, PokemonUsuario2Id) REFERENCES Pokedex_Pokemon(PokedexId, PokemonId),
    
    FOREIGN KEY (GanadorId) REFERENCES Usuarios(Id)
);

-- Creación de la tabla PeticionesEnfermeria
CREATE TABLE PeticionesEnfermeria (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EntrenadorId INT NOT NULL,
    PokemonId INT NOT NULL,
    Estado VARCHAR(20) NOT NULL CHECK (Estado IN ('Pendiente', 'En Proceso', 'Atendido')),
    FechaPeticion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EntrenadorId) REFERENCES Usuarios(Id),
    FOREIGN KEY (PokemonId) REFERENCES Pokemones(Id)
);


-- Inserción de roles iniciales
INSERT INTO Roles (Nombre) VALUES ('Entrenador'), ('Administrador'), ('Enfermero');

INSERT INTO Pokemones (Nombre, Tipo, Debilidad, HP, Ataque, Defensa)
VALUES 
('Bulbasaur', 'Planta', 'Fuego', 45, 49, 49),
('Charmander', 'Fuego', 'Agua', 39, 52, 43),
('Squirtle', 'Agua', 'Eléctrico', 44,  48, 65),
('Dragonair', 'Dragón', 'Hielo', 61, 84, 65),
('Chikorita', 'Planta', 'Fuego', 45, 49, 65),
('Cyndaquil', 'Fuego', 'Agua', 39, 52, 43),
('Totodile', 'Agua', 'Eléctrico', 50, 65, 64),
('Umbreon', 'Siniestro', 'Lucha', 95, 65, 110),
('Treecko', 'Planta', 'Fuego', 40, 45, 35),
('Torchic', 'Fuego', 'Agua', 45, 60, 40),
('Mudkip', 'Agua', 'Eléctrico', 50, 70, 50),
('Beautifly', 'Volador', 'Fuego', 60, 70, 50);

-- Inserción de columnas para el funcionamiendo de la enfermería
ALTER TABLE Pokemones
ADD Imagen NVARCHAR(255) NULL;

UPDATE Pokemones
SET Imagen = CONCAT('Pokemon', Id, '.png')
WHERE Id BETWEEN 1 AND 12;

ALTER TABLE Pokedex_Pokemon
ADD Estado NVARCHAR(100) NULL;

