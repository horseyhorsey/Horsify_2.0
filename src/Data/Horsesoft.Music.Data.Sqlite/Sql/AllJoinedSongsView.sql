AS SELECT        S.Id, S.Rating, S.Year, Art.Name AS Artist, S.Title, S.Time, S.Country, A.Title AS Album, M.MusicKey, S.Bpm, S.BitRate, S.Track, S.Comment, L.Name AS Label, G.Name AS Genre, 
                         I.DriveVolume || I.Folder || '\' || I.Filename AS FileLocation, S.DiscogId, D.ReleaseId, S.ImageLocation, S.TimesPlayed, S.IsDamaged, S.AddedDate, S.LastPlayed, S.NSFW
FROM            Song AS S INNER JOIN
                         [File] AS I ON I.Id = S.FileId INNER JOIN
                         Album AS A ON A.Id = S.AlbumId INNER JOIN
                         Label AS L ON L.Id = S.LabelId INNER JOIN
                         Genre AS G ON G.Id = S.GenreId INNER JOIN
                         MusicalKey AS M ON M.Id = S.MusicalKeyId INNER JOIN
                         Discog AS D ON D.Id = S.DiscogId INNER JOIN
                         Artist AS Art ON Art.Id = S.ArtistId;