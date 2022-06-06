export interface PageResult<T> {
  data: T[];
  skip: number;
  totalCount: number;
  take: number;
}
