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
import { actions } from "@/server/controllers";

async function DashboardStatsWrapper({ token }: { token: string }) {
  const dashboard = await actions.dashboard.getDashboardData(token);
  return <DashboardStats dashboard={dashboard} />;
}

async function DashboardChartsWrapper({ token }: { token: string }) {
  const dashboard = await actions.dashboard.getDashboardData(token);
  return <DashboardCharts dashboard={dashboard} />;
}

async function AuditLogsWrapper({ token }: { token: string }) {
  const logs = await actions.dashboard.getAuditLogs(token, 5);
  return <AuditLogsTable logs={logs} />;
}

async function RecentProductsWrapper({ token }: { token: string }) {
  const products = await actions.dashboard.getRecentProducts(token, 5);
  return <RecentProductsTable products={products} />;
}

export default async function DashboardPage() {
  const token = await actions.token.getValidAuthToken();

  return (
    <DashboardShell title="Dashboard">
      <Suspense fallback={<StatCardsSkeleton />}>
        <DashboardStatsWrapper token={token} />
      </Suspense>

      <Suspense fallback={<ChartsSkeleton />}>
        <DashboardChartsWrapper token={token} />
      </Suspense>

      <div className="grid gap-4 md:grid-cols-2 mt-4">
        <Suspense fallback={<DashboardTablesSkeleton />}>
          <AuditLogsWrapper token={token} />
        </Suspense>

        <Suspense fallback={<DashboardTablesSkeleton />}>
          <RecentProductsWrapper token={token} />
        </Suspense>
      </div>
    </DashboardShell>
  );
}
