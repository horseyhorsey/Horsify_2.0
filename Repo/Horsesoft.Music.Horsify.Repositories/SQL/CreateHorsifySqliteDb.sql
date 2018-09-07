--
-- File generated with SQLiteStudio v3.1.1 on Sat Jun 9 06:45:20 2018
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: __EFMigrationsHistory
CREATE TABLE "__EFMigrationsHistory" (



    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,



    "ProductVersion" TEXT NOT NULL



);

-- Table: Album
CREATE TABLE Album (Id INTEGER NOT NULL CONSTRAINT PK_Album PRIMARY KEY AUTOINCREMENT, Title TEXT NOT NULL UNIQUE ON CONFLICT REPLACE);

-- Table: Artist
CREATE TABLE Artist (Id INTEGER NOT NULL CONSTRAINT PK_Artist PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL UNIQUE ON CONFLICT REPLACE);

-- Table: Discog
CREATE TABLE Discog (Id INTEGER NOT NULL CONSTRAINT PK_Discog PRIMARY KEY AUTOINCREMENT, ReleaseId INTEGER NOT NULL UNIQUE ON CONFLICT REPLACE);

-- Table: File
CREATE TABLE File (Id INTEGER NOT NULL CONSTRAINT PK_File PRIMARY KEY AUTOINCREMENT, DriveVolume TEXT (1, 5) NOT NULL, FileName TEXT NOT NULL, Folder TEXT (0) NOT NULL, Hash TEXT UNIQUE);

-- Table: Filter
CREATE TABLE Filter (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE, SearchTerms TEXT);
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (1, '1960s', 'Year:196%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (2, '1970s', 'Year:197%');

-- Table: Genre
CREATE TABLE Genre (Id INTEGER NOT NULL CONSTRAINT PK_Genre PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL UNIQUE ON CONFLICT REPLACE, Style TEXT);

-- Table: Label
CREATE TABLE Label (Id INTEGER NOT NULL CONSTRAINT PK_Label PRIMARY KEY AUTOINCREMENT, DiscogsId INTEGER, Name TEXT NOT NULL UNIQUE ON CONFLICT REPLACE);

-- Table: MusicalKey
CREATE TABLE MusicalKey (Id INTEGER NOT NULL CONSTRAINT PK_MusicalKey PRIMARY KEY AUTOINCREMENT, MusicKey TEXT (1, 12) NOT NULL UNIQUE ON CONFLICT REPLACE);

-- Table: Playlist
CREATE TABLE Playlist (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE, Items TEXT, Count INTEGER);

-- Table: Song
CREATE TABLE Song (Id INTEGER NOT NULL CONSTRAINT PK_Song PRIMARY KEY AUTOINCREMENT, AddedDate DATETIME, AlbumId INTEGER, ArtistId INTEGER, BitRate INTEGER, Bpm INTEGER, Comment TEXT, Country TEXT, DiscogId INTEGER, FileId INTEGER NOT NULL, GenreId INTEGER, ImageLocation TEXT, IsDamaged BOOLEAN, LabelId INTEGER, LastPlayed DATETIME, MusicalKeyId INTEGER, NSFW BOOLEAN, Rating INTEGER, Time TIME, TimesPlayed INTEGER, Title TEXT, Track INTEGER, Year INTEGER, CONSTRAINT FK_Song_Album_AlbumId FOREIGN KEY (AlbumId) REFERENCES Album (Id) ON DELETE RESTRICT, CONSTRAINT FK_Song_Artist_ArtistId FOREIGN KEY (ArtistId) REFERENCES Artist (Id) ON DELETE RESTRICT, CONSTRAINT FK_Song_Discog_DiscogId FOREIGN KEY (DiscogId) REFERENCES Discog (Id) ON DELETE RESTRICT, CONSTRAINT FK_Song_File_FileId FOREIGN KEY (FileId) REFERENCES File (Id) ON DELETE RESTRICT, CONSTRAINT FK_Song_Genre_GenreId FOREIGN KEY (GenreId) REFERENCES Genre (Id) ON DELETE RESTRICT, CONSTRAINT FK_Song_Label_LabelId FOREIGN KEY (LabelId) REFERENCES Label (Id) ON DELETE RESTRICT, CONSTRAINT FK_Song_MusicalKey_MusicalKeyId FOREIGN KEY (MusicalKeyId) REFERENCES MusicalKey (Id) ON DELETE RESTRICT);

-- Table: Status
CREATE TABLE "Status" (



    "Id" INTEGER NOT NULL CONSTRAINT "PK_Status" PRIMARY KEY AUTOINCREMENT,



    "Status" TEXT (1, 15) NOT NULL



);

-- Index: IX_File_Hash
CREATE UNIQUE INDEX IX_File_Hash ON File ("Hash");

-- Index: IX_Song_AlbumId
CREATE INDEX IX_Song_AlbumId ON Song ("AlbumId");

-- Index: IX_Song_ArtistId
CREATE INDEX IX_Song_ArtistId ON Song ("ArtistId");

-- Index: IX_Song_DiscogId
CREATE INDEX IX_Song_DiscogId ON Song ("DiscogId");

-- Index: IX_Song_FileId
CREATE INDEX IX_Song_FileId ON Song ("FileId");

-- Index: IX_Song_GenreId
CREATE INDEX IX_Song_GenreId ON Song ("GenreId");

-- Index: IX_Song_LabelId
CREATE INDEX IX_Song_LabelId ON Song ("LabelId");

-- Index: IX_Song_MusicalKeyId
CREATE INDEX IX_Song_MusicalKeyId ON Song ("MusicalKeyId");

-- Index: UQ_FileMatch
CREATE UNIQUE INDEX UQ_FileMatch ON File (DriveVolume, FileName, Folder);

-- View: AllJoinedTables
CREATE VIEW AllJoinedTables AS SELECT S.Id, I.DriveVolume || I.Folder || '\' || I.Filename AS FileLocation, S.Rating, S.Year, Art.Name AS Artist, S.Title, S.Time, S.Country, A.Title AS Album, M.MusicKey, S.Bpm, S.BitRate, S.Track, S.Comment, L.Name AS Label, G.Name AS Genre, 
                          S.DiscogId, D.ReleaseId, S.ImageLocation, S.TimesPlayed, S.IsDamaged, S.AddedDate, S.LastPlayed, S.NSFW
FROM                     [File] AS I 
                          LEFT JOIN [Song] AS S ON I.Id = S.FileId
                          LEFT JOIN Album AS A ON A.Id = S.AlbumId
                          LEFT JOIN Label AS L ON L.Id = S.LabelId
                          LEFT JOIN Genre AS G ON G.Id = S.GenreId
                          LEFT JOIN MusicalKey AS M ON M.Id = S.MusicalKeyId
                          LEFT JOIN Discog AS D ON D.Id = S.DiscogId
                          LEFT JOIN Artist AS Art ON Art.Id = S.ArtistId;

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
