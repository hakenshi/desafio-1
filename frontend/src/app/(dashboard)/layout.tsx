import { Header } from "@/components/header";
import Sidebar from "@/components/sidebar";
import { actions } from "@/server/controllers";
import { PropsWithChildren } from "react";

export default async function DashboardLayout({ children }: PropsWithChildren) {

    const user = await actions.auth.getUserInfo()

    return (
        <div className="grid h-screen grid-rows-[auto_1fr] grid-cols-[12rem_auto]">
            <Header user={user} />
            <Sidebar />
            <main className="row-start-2 col-start-2 overflow-y-scroll">
                {children}
            </main>
        </div>
    )
}