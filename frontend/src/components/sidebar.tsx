import { HomeIcon, Settings, ShoppingBag, StoreIcon, Users } from "lucide-react";
import Link from "next/link";
import { actions } from "@/server/controllers";

export default async function Sidebar() {
    const user = await actions.auth.getUserInfo()
    const isAdmin = user.role === "admin"

    return (
        <aside className="bg-card border-r row-start-2 col-start-1">
            <nav className="space-y-8 px-5 py-2 flex flex-col justify-center h-8/12">
                <div>
                    <p className="text-muted-foreground font-semibold text-sm">General</p>
                    <ul className="pt-2">
                        <li className="hover:bg-accent p-2 rounded-md">
                            <Link className="flex gap-2 text-foreground hover:text-primary" href={"/dashboard"} >
                                <HomeIcon />
                                <p>Dashboard</p>
                            </Link>
                        </li>
                    </ul>
                </div>
                <div>
                    <p className="text-muted-foreground font-semibold text-sm">Shop</p>
                    <ul className="pt-2">
                        <li className="hover:bg-accent p-2 rounded-md">
                            <Link className="flex gap-2 text-foreground hover:text-primary" href={"/products"} >
                                <ShoppingBag />
                                <p>Products</p>
                            </Link>
                        </li>
                        <li className="hover:bg-accent p-2 rounded-md">
                            <Link className="flex gap-2 text-foreground hover:text-primary" href={"/categories"} >
                                <StoreIcon />
                                <p>Categories</p>
                            </Link>
                        </li>
                        {isAdmin && (
                            <li className="hover:bg-accent p-2 rounded-md">
                                <Link className="flex gap-2 text-foreground hover:text-primary" href={"/users"} >
                                    <Users />
                                    <p>Users</p>
                                </Link>
                            </li>
                        )}
                    </ul>
                </div>
                <div>
                    <p className="text-muted-foreground font-semibold text-sm">Support</p>
                    <ul className="pt-2">
                        <li className="hover:bg-accent p-2 rounded-md">
                            <Link className="flex gap-2 text-foreground hover:text-primary" href={"/settings"} >
                                <Settings />
                                <p>Settings</p>
                            </Link>
                        </li>
                    </ul>
                </div>
            </nav>
        </aside>
    )
}