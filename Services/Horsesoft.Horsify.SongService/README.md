# Horsify WCF Service

### IHorsifySongService

This is the main interface that inherits the rest.

- IHorsifyFileService
- IHorsifyFilterService
- IHorsifyPlaylistService
- IHorsifySearchService
- IHorsifyTagService

#### IHorsifySearchService

    /// Searches with like wildcards from all tables given in the searchtypes flags. <para/>
    /// Search wild cards joined with ; eg "%Noisia%|%Jackson"
	SearchLike()


#### IHorsifyFileService

		"NOT USED!?"
		int Add(File file)

		//Gets a file from ID
		File GetById(long value)

#### IHorsifyFilterService

		//Gets all filters
        IEnumerable<Filter> GetFilters();

		//Create a new filter (insert)
        void InsertFilter(Filter filter);

		//Remove a filter
        void RemoveFilter(Filter filter);

		//Updates a filter
        void UpdateFilter(Filter filter);

	

