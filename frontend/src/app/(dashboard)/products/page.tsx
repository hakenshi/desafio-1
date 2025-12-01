import DashboardShell from "@/components/dashboard/dashboard-shell"
import { DataTable } from "@/components/ui/data-table"
import { columns } from "./columns"
import { getAllProducts } from "@/server/controllers/product.controller"

interface ProductsPageProps {
  searchParams: Promise<{ page?: string; pageSize?: string }>
}

export default async function ProductsPage({ searchParams }: ProductsPageProps) {
  const params = await searchParams
  const page = Number(params.page) || 1
  const pageSize = Number(params.pageSize) || 10

  const result = await getAllProducts({ page, pageSize })

  return (
    <DashboardShell title="Products">
      <DataTable 
        columns={columns} 
        data={result.items}
        pagination={{
          page: result.page,
          pageSize: result.pageSize,
          totalCount: result.totalCount,
          totalPages: result.totalPages,
          hasPreviousPage: result.hasPreviousPage,
          hasNextPage: result.hasNextPage,
        }}
        searchKey="name"
        searchPlaceholder="Search products..."
      />
    </DashboardShell>
  )
}
