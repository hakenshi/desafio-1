import { Suspense } from "react";
import DashboardShell from "@/components/dashboard/dashboard-shell";
import { DataTable } from "@/components/ui/data-table";
import { columns } from "./columns";
import { getAllProducts } from "@/server/controllers/product.controller";
import { TableSkeleton } from "@/components/dashboard/table-skeleton";

interface ProductsPageProps {
  searchParams: Promise<{ page?: string; pageSize?: string }>;
}

async function ProductsTable({ page, pageSize }: { page: number; pageSize: number }) {
  const result = await getAllProducts({ page, pageSize });

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
      searchPlaceholder="Search products..."
    />
  );
}

export default async function ProductsPage({ searchParams }: ProductsPageProps) {
  const params = await searchParams;
  const page = Number(params.page) || 1;
  const pageSize = Number(params.pageSize) || 10;

  return (
    <DashboardShell title="Products">
      <Suspense fallback={<TableSkeleton columns={6} rows={pageSize} />}>
        <ProductsTable page={page} pageSize={pageSize} />
      </Suspense>
    </DashboardShell>
  );
}
