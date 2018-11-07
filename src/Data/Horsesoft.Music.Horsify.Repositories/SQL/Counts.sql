

-- Gets stats for database entries

SELECT COUNT(F.Id) as FileCount, 
       COUNT([Song].Id) as Songs, 
       COUNT(DISTINCT ArtistId) as Artists, 
       COUNT(DISTINCT AlbumId) as Albums, 
       COUNT(DISTINCT Country) as Countries, 
       COUNT(DISTINCT DiscogId) as Discogs,
       COUNT(DISTINCT GenreId) as Genres,
       COUNT(DISTINCT ImageLocation) as Images,
       COUNT(DISTINCT LabelId) as Labels,
       COUNT(DISTINCT Title) as Titles
    From [File] as F  
   Left Join [Song] On [Song].Id = F.Id;

