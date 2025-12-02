"use client"

import { ColumnDef } from "@tanstack/react-table"
import { MoreHorizontal, ArrowUpDown } from "lucide-react"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { ProductModel } from "@/server/models/product.model"
import { useState } from "react"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import ProductsForm from "@/components/forms/products-form"
import { actions } from "@/server/controllers"
import { useRouter } from "next/navigation"
import { Spinner } from "@/components/ui/spinner"

export type Product = ProductModel.Product

function ProductActions({ product }: { product: Product }) {
  const router = useRouter()
  const [showEditDialog, setShowEditDialog] = useState(false)
  const [showDeleteDialog, setShowDeleteDialog] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)

  const handleDelete = async () => {
    setIsDeleting(true)
    try {
      await actions.product.deleteProduct(product.id)
      router.refresh()
      setShowDeleteDialog(false)
    } catch (error) {
      console.error("Failed to delete product:", error)
    } finally {
      setIsDeleting(false)
    }
  }

  return (
    <>
      <DropdownMenu modal={false}>
        <DropdownMenuTrigger asChild>
          <Button variant="ghost" className="h-8 w-8 p-0">
            <span className="sr-only">Open menu</span>
            <MoreHorizontal className="h-4 w-4" />
          </Button>
        </DropdownMenuTrigger>
        <DropdownMenuContent align="end">
          <DropdownMenuLabel>Actions</DropdownMenuLabel>
          <DropdownMenuItem
            onClick={() => navigator.clipboard.writeText(product.id)}
          >
            Copy product ID
          </DropdownMenuItem>
          <DropdownMenuSeparator />
          <DropdownMenuItem onSelect={() => setShowEditDialog(true)}>
            Edit product
          </DropdownMenuItem>
          <DropdownMenuItem onSelect={() => setShowDeleteDialog(true)} className="text-destructive">
            Delete product
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
      <Dialog open={showEditDialog} onOpenChange={setShowEditDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Product</DialogTitle>
          </DialogHeader>
          <ProductsForm 
            product={product} 
            onSuccess={() => setShowEditDialog(false)} 
          />
        </DialogContent>
      </Dialog>
      <Dialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Delete Product</DialogTitle>
            <DialogDescription>
              Are you sure you want to delete &quot;{product.name}&quot;? This action cannot be undone.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowDeleteDialog(false)} disabled={isDeleting}>
              Cancel
            </Button>
            <Button variant="destructive" onClick={handleDelete} disabled={isDeleting}>
              {isDeleting ? <><Spinner /> Deleting...</> : "Delete"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  )
}

export const columns: ColumnDef<Product>[] = [
  {
    accessorKey: "sku",
    header: "SKU",
    cell: ({ row }) => {
      const sku = row.getValue("sku") as string
      return <span className="font-mono text-xs">{sku}</span>
    },
  },
  {
    accessorKey: "name",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Name
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      )
    },
  },
  {
    accessorKey: "description",
    header: "Description",
    cell: ({ row }) => {
      const description = row.getValue("description") as string
      return (
        <span className="max-w-[200px] truncate block">
          {description}
        </span>
      )
    },
  },
  {
    accessorKey: "price",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Price
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      )
    },
    cell: ({ row }) => {
      const price = parseFloat(row.getValue("price"))
      const formatted = new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
      }).format(price)
      return formatted
    },
  },
  {
    accessorKey: "stockQuantity",
    header: "Stock",
    cell: ({ row }) => {
      const stock = row.getValue("stockQuantity") as number
      return (
        <span className={stock < 10 ? "text-destructive font-medium" : ""}>
          {stock}
        </span>
      )
    },
  },
  {
    id: "totalValue",
    header: "Total Value",
    cell: ({ row }) => {
      const price = row.original.price
      const stock = row.original.stockQuantity
      const total = price * stock
      return new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
      }).format(total)
    },
  },
  {
    accessorKey: "categoryName",
    header: "Category",
    filterFn: (row, id, filterValue) => {
      return row.getValue(id) === filterValue
    },
  },
  {
    accessorKey: "isLowStock",
    header: "Status",
    cell: ({ row }) => {
      const isLowStock = row.getValue("isLowStock") as boolean
      return (
        <span className={`px-2 py-1 rounded-md text-xs ${isLowStock ? "bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400" : "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400"}`}>
          {isLowStock ? "Low Stock" : "In Stock"}
        </span>
      )
    },
  },
  {
    accessorKey: "createdAt",
    header: "Created At",
    cell: ({ row }) => {
      const date = row.getValue("createdAt") as string
      return new Date(date).toISOString().split("T")[0]
    },
  },
  {
    id: "actions",
    cell: ({ row }) => <ProductActions product={row.original} />,
  },
]
