import { PropsWithChildren } from "react";

interface Props extends PropsWithChildren{
    title: string
}

export default function DashboardShell({title, children}: Props){
    return (
        <div className="p-5">
            <main className="bg-white p-5 rounded-md h-full">
                <p className="text-lg font-semibold pb-5">{title}</p>
                {children}
            </main>
        </div>
    )
}