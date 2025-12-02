import { Suspense } from "react";
import DashboardShell from "@/components/dashboard/dashboard-shell";
import { DataTable } from "@/components/ui/data-table";
import { columns } from "./columns";
import { TableSkeleton } from "@/components/dashboard/table-skeleton";
import { actions } from "@/server/controllers";
import ProductsForm from "@/components/forms/products-form";

interface ProductsPageProps {
  searchParams: Promise<{ page?: string; pageSize?: string; categoryId?: string }>;
}

async function ProductsTable({
  page,
  pageSize,
  categoryId,
}: {
  page: number;
  pageSize: number;
  categoryId?: string;
}) {
  const token = await actions.token.getValidAuthToken();

  const [products, categories] = await Promise.all([
    actions.product.getAllProducts(token, { page, pageSize, categoryId }),
    actions.category.getAllCategories(token, { page: 1, pageSize: 100 }),
  ]);

  return (
    <DataTable
      columns={columns}
      data={products.items}
      pagination={{
        page: products.page,
        pageSize: products.pageSize,
        totalCount: products.totalCount,
        totalPages: products.totalPages,
        hasPreviousPage: products.hasPreviousPage,
        hasNextPage: products.hasNextPage,
      }}
      filterKey="categoryId"
      filterOptions={categories.items.map((c) => ({ label: c.name, value: c.id }))}
      filterPlaceholder="Category"
      currentFilter={categoryId}
      searchKey="name"
      searchPlaceholder="Search products..."
    >
      <ProductsForm />
    </DataTable>
  );
}

export default async function ProductsPage({ searchParams }: ProductsPageProps) {
  const params = await searchParams;
  const page = Number(params.page) || 1;
  const pageSize = Number(params.pageSize) || 10;
  const categoryId = params.categoryId;

  return (
    <DashboardShell title="Products">
      <Suspense fallback={<TableSkeleton columns={10} rows={pageSize} />}>
        <ProductsTable page={page} pageSize={pageSize} categoryId={categoryId} />
      </Suspense>
    </DashboardShell>
  );
}
