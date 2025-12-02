import { Header } from "@/components/header";
import Sidebar from "@/components/sidebar";
import { actions } from "@/server/controllers";
import { PropsWithChildren, Suspense } from "react";
import { Spinner } from "@/components/ui/spinner";
import { DashboardWrapper } from "@/components/dashboard/dashboard-wrapper";

async function DashboardContent({ children }: PropsWithChildren) {
  const token = await actions.token.getValidAuthToken();
  const user = await actions.auth.getUserInfo(token);

  return (
    <DashboardWrapper>
      <Header user={user} />
      <Sidebar user={user} />
      <main className="lg:row-start-2 lg:col-start-2 overflow-y-auto bg-muted/30 min-h-[calc(100vh-57px)] lg:min-h-0">
        {children}
      </main>
    </DashboardWrapper>
  );
}

export default function DashboardLayout({ children }: PropsWithChildren) {
  return (
    <Suspense
      fallback={
        <div className="flex items-center justify-center h-screen">
          <Spinner />
        </div>
      }
    >
      <DashboardContent>{children}</DashboardContent>
    </Suspense>
  );
}
