--
-- File generated with SQLiteStudio v3.1.1 on Wed Aug 1 15:32:19 2018
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Filter
CREATE TABLE Filter (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE, SearchTerms TEXT);
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (1, 'D&B', 'Genre:%Neurofunk%;%Techstep%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (2, 'Early80s', 'Year:1981;1982;1983;1984');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (3, 'Boogie', 'Genre:%Boogie%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (4, 'RockPunk', 'Genre:%Punk%;%Indie Rock%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (5, 'Noisia', 'Artist:%Noisia%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (13, 'Nineties', 'Year:199%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (14, 'SuesTunes', 'Artist:%Houston%;%jackson%;%boyz%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (15, 'RockStd', 'Genre:%Rock%;%punk%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (16, 'Rockabilly', 'Genre:%Rockabilly%;%psychobilly%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (17, 'Classic Rock', 'Genre:%Classic Rock%;%metal%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (18, 'grandflash', 'Artist:%Grandmaster%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (19, 'Sixties', 'Year:196%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (20, 'Jungle', 'Genre:Jungle');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (21, 'Jackson', 'Artist:%jackson%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (22, 'Funk', 'Genre:%funk%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (23, 'Grunge', 'Genre:%grunge%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (24, 'Janet', 'Artist:%janet%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (25, 'Hardcore Jungle', 'Genre:%Hardcore, Jungle%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (26, 'Recent', 'Year:2018;2017;2016;2015');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (27, 'Disco', 'Genre:%Disco%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (28, 'Drum&Bass', 'Genre:%Drum & Bass%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (29, 'Hip Hop', 'Genre:%Hip, Hop%;%Hip-Hop%;%Hip Hop%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (30, 'Jacksons', 'Artist:%jackson five%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (31, 'Pop', 'Genre:%Pop%');
INSERT INTO Filter (Id, Name, SearchTerms) VALUES (32, 'Noughties', 'Year:201%');

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;