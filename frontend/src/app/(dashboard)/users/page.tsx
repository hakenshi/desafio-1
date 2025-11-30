import DashboardShell from "@/components/dashboard/dashboard-shell"
import { DataTable } from "@/components/ui/data-table"
import { columns } from "./columns"
import { getUserInfo } from "@/server/controllers/auth.controller"

export default async function UsersPage() {
  // Por enquanto, mostra apenas o usuário atual
  // TODO: Criar endpoint no backend para listar todos os usuários (admin only)
  const currentUser = await getUserInfo()
  const users = [currentUser]

  return (
    <DashboardShell title="Users">
      <div className="mb-4 p-4 bg-blue-50 border border-blue-200 rounded-md">
        <p className="text-sm text-blue-800">
          <strong>Note:</strong> Currently showing only your user profile. 
          A full user management endpoint needs to be implemented in the backend.
        </p>
      </div>
      <DataTable 
        columns={columns} 
        data={users}
        searchKey="username"
        searchPlaceholder="Search users..."
      />
    </DashboardShell>
  )
}
