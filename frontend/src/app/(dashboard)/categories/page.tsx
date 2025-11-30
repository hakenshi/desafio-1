import DashboardShell from "@/components/dashboard/dashboard-shell"
import { DataTable } from "@/components/ui/data-table"
import { columns } from "./columns"
import { getAllCategories } from "@/server/controllers/category.controller"

export default async function CategoriesPage() {
  const categories = await getAllCategories()

  return (
    <DashboardShell title="Categories">
      <DataTable 
        columns={columns} 
        data={categories}
        searchKey="name"
        searchPlaceholder="Search categories..."
      />
    </DashboardShell>
  )
}