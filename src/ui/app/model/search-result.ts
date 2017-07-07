
export interface ISearchResult<T> {

    items: Array<T>;
    page: number;
    pageSize: number;
    total: number;

}