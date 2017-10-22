export interface IKeyValuePair<TKey, TValue> { };
export type IKeyValueList<TKey, TValue> = IKeyValuePair<TKey, TValue>[];
export type KeyValue = { key: string, value: string };