import { Suspense } from "react";
import DashboardShell from "@/components/dashboard/dashboard-shell";
import { DataTable } from "@/components/ui/data-table";
import { columns } from "./columns";
import { getAllCategories } from "@/server/controllers/category.controller";
import { TableSkeleton } from "@/components/dashboard/table-skeleton";

interface CategoriesPageProps {
  searchParams: Promise<{ page?: string; pageSize?: string }>;
}

async function CategoriesTable({ page, pageSize }: { page: number; pageSize: number }) {
  const result = await getAllCategories({ page, pageSize });

  return (
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
      searchPlaceholder="Search categories..."
    />
  );
}

export default async function CategoriesPage({ searchParams }: CategoriesPageProps) {
  const params = await searchParams;
  const page = Number(params.page) || 1;
  const pageSize = Number(params.pageSize) || 10;

  return (
    <DashboardShell title="Categories">
      <Suspense fallback={<TableSkeleton columns={4} rows={pageSize} />}>
        <CategoriesTable page={page} pageSize={pageSize} />
      </Suspense>
    </DashboardShell>
  );
}
