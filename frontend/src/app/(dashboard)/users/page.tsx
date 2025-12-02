import { Suspense } from "react";
import DashboardShell from "@/components/dashboard/dashboard-shell";
import { DataTable } from "@/components/ui/data-table";
import { columns } from "./columns";
import { redirect } from "next/navigation";
import { TableSkeleton } from "@/components/dashboard/table-skeleton";
import UserForm from "@/components/forms/user-form";
import { actions } from "@/server/controllers";
import { AuthModel } from "@/server/models/auth.model";

interface UsersPageProps {
  searchParams: Promise<{ role?: string }>;
}

async function UsersTable({ roleFilter }: { roleFilter?: string }) {
  const token = await actions.token.getValidAuthToken();
  const currentUser = await actions.auth.getUserInfo(token);

  // Only admin can view users list
  if (currentUser.role !== "admin") {
    redirect("/dashboard");
  }

  let users: AuthModel.KeycloakUser[] = [];
  let error = null;

  try {
    users = await actions.auth.getUsers(token);
  } catch (e) {
    error = e instanceof Error ? e.message : "Failed to load users";
    users = [{ ...currentUser, enabled: true }];
  }

  // Apply role filter client-side
  const filteredUsers = roleFilter
    ? users.filter((user) => user.role === roleFilter)
    : users;

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
        data={filteredUsers}
        searchKey="username"
        searchPlaceholder="Search users..."
        filterKey="role"
        filterOptions={[
          { label: "Admin", value: "admin" },
          { label: "Manager", value: "manager" },
          { label: "User", value: "user" },
        ]}
        filterPlaceholder="Role"
        currentFilter={roleFilter}
      >
        <UserForm />
      </DataTable>
    </>
  );
}

export default async function UsersPage({ searchParams }: UsersPageProps) {
  const params = await searchParams;
  const roleFilter = params.role;

  return (
    <DashboardShell title="Users">
      <Suspense fallback={<TableSkeleton columns={5} rows={10} />}>
        <UsersTable roleFilter={roleFilter} />
      </Suspense>
    </DashboardShell>
  );
}
