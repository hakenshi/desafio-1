import { Suspense } from "react";
import DashboardShell from "@/components/dashboard/dashboard-shell";
import {
  DashboardStats,
  DashboardCharts,
  AuditLogsTable,
  RecentProductsTable,
} from "@/components/dashboard/dashboard-content";
import {
  StatCardsSkeleton,
  ChartsSkeleton,
  DashboardTablesSkeleton,
} from "@/components/dashboard/dashboard-skeletons";
import {
  getDashboardData,
  getAuditLogs,
  getRecentProducts,
} from "@/server/controllers/dashboard.controller";

async function DashboardStatsWrapper() {
  const dashboard = await getDashboardData();
  return <DashboardStats dashboard={dashboard} />;
}

async function DashboardChartsWrapper() {
  const dashboard = await getDashboardData();
  return <DashboardCharts dashboard={dashboard} />;
}

async function AuditLogsWrapper() {
  const logs = await getAuditLogs(5);
  return <AuditLogsTable logs={logs} />;
}

async function RecentProductsWrapper() {
  const products = await getRecentProducts(5);
  return <RecentProductsTable products={products} />;
}

export default function DashboardPage() {
  return (
    <DashboardShell title="Dashboard">
      <Suspense fallback={<StatCardsSkeleton />}>
        <DashboardStatsWrapper />
      </Suspense>

      <Suspense fallback={<ChartsSkeleton />}>
        <DashboardChartsWrapper />
      </Suspense>

      <div className="grid gap-4 md:grid-cols-2 mt-4">
        <Suspense fallback={<DashboardTablesSkeleton />}>
          <AuditLogsWrapper />
        </Suspense>

        <Suspense fallback={<DashboardTablesSkeleton />}>
          <RecentProductsWrapper />
        </Suspense>
      </div>
    </DashboardShell>
  );
}
