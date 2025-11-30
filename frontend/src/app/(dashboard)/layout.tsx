import { Header } from "@/components/header";
import Sidebar from "@/components/sidebar";
import { PropsWithChildren } from "react";

export default function DashboardLayout({ children }: PropsWithChildren) {
    return (
        <div>
            <Header />
            <Sidebar />
            <main>
                {children}
            </main>
        </div>
    )
}