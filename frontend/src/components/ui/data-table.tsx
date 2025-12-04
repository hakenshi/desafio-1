"use client"

import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
  SortingState,
  getSortedRowModel,
  ColumnFiltersState,
  getFilteredRowModel,
} from "@tanstack/react-table"

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "./table"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { cloneElement, isValidElement, PropsWithChildren, ReactElement, useEffect, useMemo, useState } from "react"
import { useRouter, useSearchParams } from "next/navigation"
import { Check, ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight, Filter, PlusIcon, X } from "lucide-react"
import { Dialog, DialogContent, DialogHeader, DialogTrigger } from "./dialog"
import { DialogTitle } from "@radix-ui/react-dialog"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./dropdown-menu"

interface PaginationInfo {
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasPreviousPage: boolean
  hasNextPage: boolean
}

interface FilterOption {
  label: string
  value: string
}

interface DataTableProps<TData, TValue> extends PropsWithChildren {
  columns: ColumnDef<TData, TValue>[]
  data: TData[]
  pagination?: PaginationInfo
  searchKey?: string
  searchPlaceholder?: string
  filterKey?: string
  filterOptions?: FilterOption[]
  filterPlaceholder?: string
  currentFilter?: string
}

export function DataTable<TData, TValue>({
  columns,
  data,
  pagination,
  searchKey,
  searchPlaceholder = "Search...",
  children,
  filterKey,
  filterOptions,
  filterPlaceholder = "Filter by...",
  currentFilter,
}: DataTableProps<TData, TValue>) {
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [filterSearch, setFilterSearch] = useState("")
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false)
  const router = useRouter()
  const searchParams = useSearchParams()

  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    onSortingChange: setSorting,
    getSortedRowModel: getSortedRowModel(),
    onColumnFiltersChange: setColumnFilters,
    getFilteredRowModel: getFilteredRowModel(),
    manualPagination: !!pagination,
    pageCount: pagination?.totalPages ?? -1,
    state: {
      sorting,
      columnFilters,
    },
  })

  useEffect(() => {
    if (!pagination) return

    const { page, totalPages } = pagination
    let targetPage: number | null = null

    if (page < 1) {
      targetPage = 1
    } else if (totalPages > 0 && page > totalPages) {
      targetPage = totalPages
    } else if (data.length === 0 && page > 1) {
      targetPage = page - 1
    }

    if (targetPage !== null) {
      const params = new URLSearchParams(searchParams.toString())
      params.set("page", targetPage.toString())
      router.replace(`?${params.toString()}`)
    }
  }, [pagination, data.length, router, searchParams])

  const handlePageChange = (newPage: number) => {
    if (!pagination) return
    
    const clampedPage = Math.max(1, Math.min(newPage, pagination.totalPages || 1))
    const params = new URLSearchParams(searchParams.toString())
    params.set("page", clampedPage.toString())
    router.push(`?${params.toString()}`)
  }

  const handleFilterSelect = (value: string) => {
    const params = new URLSearchParams(searchParams.toString())
    if (value) {
      params.set(filterKey || "filter", value)
      params.set("page", "1") // Reset to page 1 when filtering
    } else {
      params.delete(filterKey || "filter")
    }
    router.push(`?${params.toString()}`)
    setFilterSearch("")
  }

  const filteredOptions = useMemo(() => {
    if (!filterOptions) return []
    if (!filterSearch) return filterOptions
    return filterOptions.filter(opt => 
      opt.label.toLowerCase().includes(filterSearch.toLowerCase())
    )
  }, [filterOptions, filterSearch])

  const currentFilterLabel = currentFilter 
    ? filterOptions?.find(o => o.value === currentFilter)?.label 
    : null

  return (
    <div className="space-y-4">
      {searchKey && (
        <div className="flex flex-col sm:flex-row gap-2">
          <Input
            placeholder={searchPlaceholder}
            value={(table.getColumn(searchKey)?.getFilterValue() as string) ?? ""}
            onChange={(event) =>
              table.getColumn(searchKey)?.setFilterValue(event.target.value)
            }
            className="w-full sm:flex-1"
          />
          
          <div className="flex gap-2">
            {filterKey && filterOptions && (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button variant="outline" className="gap-2 flex-1 sm:flex-none sm:min-w-[140px]">
                    <Filter className="h-4 w-4" />
                    <span className="truncate">{currentFilterLabel || filterPlaceholder}</span>
                    {currentFilter && (
                      <X 
                        className="h-3 w-3 ml-1 hover:text-destructive shrink-0" 
                        onClick={(e) => {
                          e.stopPropagation()
                          handleFilterSelect("")
                        }}
                      />
                    )}
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end" className="w-56">
                  <div className="p-2">
                    <Input
                      placeholder="Search..."
                      value={filterSearch}
                      onChange={(e) => setFilterSearch(e.target.value)}
                      className="h-8"
                    />
                  </div>
                  <div className="max-h-48 overflow-y-auto">
                    {currentFilter && (
                      <DropdownMenuItem onClick={() => handleFilterSelect("")}>
                        <span className="text-muted-foreground">Clear filter</span>
                      </DropdownMenuItem>
                    )}
                    {filteredOptions.length > 0 ? (
                      filteredOptions.map((option) => (
                        <DropdownMenuItem
                          key={option.value}
                          onClick={() => handleFilterSelect(option.value)}
                          className="flex items-center justify-between"
                        >
                          {option.label}
                          {currentFilter === option.value && (
                            <Check className="h-4 w-4" />
                          )}
                        </DropdownMenuItem>
                      ))
                    ) : (
                      <div className="p-2 text-sm text-muted-foreground text-center">
                        No options found
                      </div>
                    )}
                  </div>
                </DropdownMenuContent>
              </DropdownMenu>
            )}

            <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
              <DialogTrigger asChild>
                <Button>
                  <PlusIcon />
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-[95vw] sm:max-w-lg">
                <DialogHeader>
                  <DialogTitle className="sr-only">Create new Resource form</DialogTitle>
                </DialogHeader>
                {isValidElement(children) 
                  ? cloneElement(children as ReactElement<{ onSuccess?: () => void }>, { 
                      onSuccess: () => setIsCreateDialogOpen(false) 
                    })
                  : children}
              </DialogContent>
            </Dialog>
          </div>
        </div>
      )}
      <div className="rounded-md border overflow-x-auto">
        <Table className="min-w-[600px]">
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  return (
                    <TableHead key={header.id}>
                      {header.isPlaceholder
                        ? null
                        : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                    </TableHead>
                  )
                })}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow
                  key={row.id}
                  data-state={row.getIsSelected() && "selected"}
                >
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={columns.length} className="h-24 text-center">
                  No results.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>

      {pagination && (
        <div className="flex flex-col sm:flex-row items-center justify-between gap-2 px-2">
          <div className="text-sm text-muted-foreground text-center sm:text-left">
            Showing {((pagination.page - 1) * pagination.pageSize) + 1} to{" "}
            {Math.min(pagination.page * pagination.pageSize, pagination.totalCount)} of{" "}
            {pagination.totalCount} results
          </div>
          <div className="flex items-center space-x-1 sm:space-x-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => handlePageChange(1)}
              disabled={!pagination.hasPreviousPage}
              className="hidden sm:flex"
            >
              <ChevronsLeft className="h-4 w-4" />
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={() => handlePageChange(pagination.page - 1)}
              disabled={!pagination.hasPreviousPage}
            >
              <ChevronLeft className="h-4 w-4" />
            </Button>
            <span className="text-sm px-2">
              {pagination.page} / {pagination.totalPages}
            </span>
            <Button
              variant="outline"
              size="sm"
              onClick={() => handlePageChange(pagination.page + 1)}
              disabled={!pagination.hasNextPage}
            >
              <ChevronRight className="h-4 w-4" />
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={() => handlePageChange(pagination.totalPages)}
              disabled={!pagination.hasNextPage}
              className="hidden sm:flex"
            >
              <ChevronsRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}
