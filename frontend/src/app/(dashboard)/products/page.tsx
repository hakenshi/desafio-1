import DashboardShell from "@/components/dashboard/dashboard-shell"
import { DataTable } from "@/components/ui/data-table"
import { columns } from "./columns"
import { getAllProducts } from "@/server/controllers/product.controller"

export default async function ProductsPage() {
  const products = await getAllProducts()

  return (
    <DashboardShell title="Products">
      <DataTable 
        columns={columns} 
        data={products}
        searchKey="name"
        searchPlaceholder="Search products..."
      />
    </DashboardShell>
  )
}
