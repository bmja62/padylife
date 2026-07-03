import type * as vue from 'vue'

type FilterMatch = boolean | number | [number, number] | [number, number][]
type DataTableCompareFunction<T = any> = (a: T, b: T) => number
type FilterFunction = (value: string, query: string, item?: InternalItem) => FilterMatch
interface DataTableHeader {
  key?: 'data-table-group' | 'data-table-select' | 'data-table-expand' | (string & object)
  value?: SelectItemKey
  title?: string
  fixed?: boolean
  align?: 'start' | 'end' | 'center'
  width?: number | string
  minWidth?: string
  maxWidth?: string
  headerProps?: Record<string, any>
  cellProps?: HeaderCellProps
  sortable?: boolean
  sort?: DataTableCompareFunction
  filter?: FilterFunction
  children?: DataTableHeader[]
}
type InternalDataTableHeader = Omit<DataTableHeader, 'key' | 'value' | 'children'> & {
  key: string | null
  value: SelectItemKey | null
  sortable: boolean
  fixedOffset?: number
  lastFixed?: boolean
  colspan?: number
  rowspan?: number
  children?: InternalDataTableHeader[]
}
declare function deepEqual(a: any, b: any): boolean
type SelectItemKey<T = Record<string, any>> = boolean | null | undefined | string | readonly (string | number)[] | ((item: T, fallback?: any) => any)
type EventProp<T extends any[] = any[], F = (...args: T) => void> = F

interface DataTableItemProps {
  items: any[]
  itemValue: SelectItemKey
  itemSelectable: SelectItemKey
  returnObject: boolean
}

type SelectionProps = Pick<DataTableItemProps, 'itemValue'> & {
  modelValue: readonly any[]
  selectStrategy: 'single' | 'page' | 'all'
  valueComparator: typeof deepEqual
  'onUpdate:modelValue': EventProp<[any[]]> | undefined
}
interface ExpandProps {
  expandOnClick: boolean
  expanded: readonly string[]
  'onUpdate:expanded': ((value: any[]) => void) | undefined
}
declare function provideSelection(props: SelectionProps, { allItems, currentPage }: {
  allItems: Ref<SelectableItem[]>
  currentPage: Ref<SelectableItem[]>
}): {
  toggleSelect: (item: SelectableItem) => void
  select: (items: SelectableItem[], value: boolean) => void
  selectAll: (value: boolean) => void
  isSelected: (items: SelectableItem | SelectableItem[]) => boolean
  isSomeSelected: (items: SelectableItem | SelectableItem[]) => boolean
  someSelected: vue.ComputedRef<boolean>
  allSelected: vue.ComputedRef<boolean>
  showSelectAll: boolean
}
declare function provideExpanded(props: ExpandProps): {
  expand: (item: DataTableItem, value: boolean) => void
  expanded: Ref<Set<string>> & {
    readonly externalValue: readonly string[]
  }
  expandOnClick: Ref<boolean>
  isExpanded: (item: DataTableItem) => boolean
  toggleExpand: (item: DataTableItem) => void
}
interface SelectableItem {
  value: any
  selectable: boolean
}
interface GroupableItem<T = any> {
  type: 'item'
  raw: T
}
interface InternalItem<T = any> {
  value: any
  raw: T
}
interface DataTableItem<T = any> extends InternalItem<T>, GroupableItem<T>, SelectableItem {
  key: any
  index: number
  columns: {
    [key: string]: any
  }
}
interface ItemSlotBase<T> {
  index: number
  item: T
  internalItem: DataTableItem<T>
  isExpanded: ReturnType<typeof provideExpanded>['isExpanded']
  toggleExpand: ReturnType<typeof provideExpanded>['toggleExpand']
  isSelected: ReturnType<typeof provideSelection>['isSelected']
  toggleSelect: ReturnType<typeof provideSelection>['toggleSelect']
}
type ItemKeySlot<T> = ItemSlotBase<T> & {
  value: any
  column: InternalDataTableHeader
}
type HeaderCellProps = Record<string, any> | ((data: Pick<ItemKeySlot<any>, 'index' | 'item' | 'internalItem' | 'value'>) => Record<string, any>)

export interface ITableHeaders {
  key?: (string & object) | 'data-table-group' | 'data-table-select' | 'data-table-expand' | undefined
  value?: SelectItemKey
  title?: string | undefined
  fixed?: boolean | undefined
  align?: 'center' | 'end' | 'start' | undefined
  editable?: boolean | undefined
  rules?: () => boolean
  formattedPrice?: boolean | undefined
  width?: string | number | undefined
  minWidth?: string | undefined
  maxWidth?: string | undefined
  headerProps?: {
    [x: string]: any
  } | undefined
  cellProps?: ((data: Pick<ItemKeySlot<any>, 'index' | 'item' | 'value' | 'internalItem'>) => Record<string, any>) | {
    [x: string]: any
  } | undefined
  sortable?: boolean | undefined
  sort?: undefined | any
  filter?: any | undefined
  children?: any[] | undefined
}
