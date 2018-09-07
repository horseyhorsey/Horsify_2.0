
SELECT COUNT(*) as Songs, COUNT(ArtistId) as Artists, COUNT(AlbumId) as Albums, COUNT(Country) as Countries, COUNT(DiscogId) as Discogs,
COUNT(DISTINCT (ImageLocation == NULL)) as NoImages
   FROM Song;
