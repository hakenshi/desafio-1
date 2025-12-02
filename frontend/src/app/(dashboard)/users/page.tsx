import { Suspense } from "react";
import DashboardShell from "@/components/dashboard/dashboard-shell";
import { DataTable } from "@/components/ui/data-table";
import { columns } from "./columns";
import { getUserInfo, getUsers } from "@/server/controllers/auth.controller";
import { redirect } from "next/navigation";
import { TableSkeleton } from "@/components/dashboard/table-skeleton";
import UserForm from "@/components/forms/user-form";

async function UsersTable() {
  const currentUser = await getUserInfo();

  // Only admin can view users list
  if (currentUser.role !== "admin") {
    redirect("/dashboard");
  }

  let users = [];
  let error = null;

  try {
    users = await getUsers();
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load users";
    users = [{ ...currentUser, enabled: true }];
  }

  return (
    <>
      {error && (
        <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-md">
          <p className="text-sm text-red-800">
            <strong>Error:</strong> {error}
          </p>
        </div>
      )}
      <DataTable
        columns={columns}
        data={users}
        searchKey="username"
        searchPlaceholder="Search users..."
        filterKey="role"
        filterOptions={[
          { label: "Admin", value: "admin" },
          { label: "Manager", value: "manager" },
          { label: "User", value: "user" },
        ]}
        filterPlaceholder="Role"
      >
        <UserForm />
      </DataTable>
    </>
  );
}

export default function UsersPage() {
  return (
    <DashboardShell title="Users">
      <Suspense fallback={<TableSkeleton columns={5} rows={10} />}>
        <UsersTable />
      </Suspense>
    </DashboardShell>
  );
}
