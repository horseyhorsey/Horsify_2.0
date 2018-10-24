SELECT F.Id, S.FileId as SongFileId, F.FileName, F.Folder

  FROM File as F
  LEFT JOIN [Song] AS S ON F.Id = S.FileId
  
where S.FileId IS NULL;