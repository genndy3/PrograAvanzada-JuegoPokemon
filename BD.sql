-- Creaci�n de la base de datos
CREATE DATABASE ProyectoPokedex;
GO

-- Selecci�n de la base de datos
USE ProyectoPokedex;
GO

-- Creaci�n de la tabla Roles
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre VARCHAR(50) NOT NULL
);

-- Creaci�n de la tabla Usuarios
CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CuentaUsuario VARCHAR(50) NOT NULL UNIQUE,
    Contrase�aHash VARCHAR(255) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    RolId INT NOT NULL,
    FOREIGN KEY (RolId) REFERENCES Roles(Id)
);

-- Creaci�n de la tabla Pokemones
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

-- Creaci�n de la tabla Mensajes
CREATE TABLE Mensajes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    RemitenteId INT NOT NULL,
    DestinatarioId INT NOT NULL,
    Contenido TEXT NOT NULL,
    FechaEnvio DATETIME DEFAULT GETDATE(),
    Estado VARCHAR(20) NOT NULL CHECK (Estado IN ('Enviado', 'Le�do')),
    FOREIGN KEY (RemitenteId) REFERENCES Usuarios(Id),
    FOREIGN KEY (DestinatarioId) REFERENCES Usuarios(Id)
);

-- Creaci�n de la tabla Retos
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

-- Creaci�n de la tabla PeticionesEnfermeria
CREATE TABLE PeticionesEnfermeria (
    Id INT PRIMARY KEY IDENTITY(1,1),
    EntrenadorId INT NOT NULL,
    PokemonId INT NOT NULL,
    Estado VARCHAR(20) NOT NULL CHECK (Estado IN ('Pendiente', 'En Proceso', 'Atendido')),
    FechaPeticion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (EntrenadorId) REFERENCES Usuarios(Id),
    FOREIGN KEY (PokemonId) REFERENCES Pokemones(Id)
);


-- Inserci�n de roles iniciales
INSERT INTO Roles (Nombre) VALUES ('Entrenador'), ('Administrador'), ('Enfermero');

INSERT INTO Pokemones (Nombre, Tipo, Debilidad, HP, Ataque, Defensa, Imagen)
VALUES 
('Bulbasaur', 'Planta', 'Fuego', 45, 49, 49, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/001.png'),
('Charmander', 'Fuego', 'Agua', 39, 52, 43, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/004.png'),
('Squirtle', 'Agua', 'El�ctrico', 44, 48, 65, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/007.png'),
('Dragonair', 'Drag�n', 'Hielo', 61, 84, 65, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/148.png'),
('Chikorita', 'Planta', 'Fuego', 45, 49, 65, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/152.png'),
('Cyndaquil', 'Fuego', 'Agua', 39, 52, 43, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/155.png'),
('Totodile', 'Agua', 'El�ctrico', 50, 65, 64, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/158.png'),
('Umbreon', 'Siniestro', 'Lucha', 95, 65, 110, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/197.png'),
('Treecko', 'Planta', 'Fuego', 40, 45, 35, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/252.png'),
('Torchic', 'Fuego', 'Agua', 45, 60, 40, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/255.png'),
('Mudkip', 'Agua', 'El�ctrico', 50, 70, 50, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/258.png'),
('Beautifly', 'Volador', 'Fuego', 60, 70, 50, 'https://www.pokemon.com/static-assets/content-assets/cms2/img/pokedex/full/267.png');

-- Inserci�n de columnas para el funcionamiendo de la enfermer�a
ALTER TABLE Pokemones
ADD Imagen NVARCHAR(255) NULL;

UPDATE Pokemones
SET Imagen = CONCAT('Pokemon', Id, '.png')
WHERE Id BETWEEN 1 AND 12;

ALTER TABLE Pokedex_Pokemon
ADD Estado NVARCHAR(100) NULL;

CREATE TABLE Usuario_Pokemones (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UsuarioId INT NOT NULL,
    PokemonId INT NOT NULL,
    FechaAsignacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    FOREIGN KEY (PokemonId) REFERENCES Pokemones(Id)
);
