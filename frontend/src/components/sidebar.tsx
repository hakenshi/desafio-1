import { HomeIcon, Settings, ShoppingBag, StoreIcon, Users } from "lucide-react";
import Link from "next/link";
import { actions } from "@/server/controllers";

export default async function Sidebar() {
    const user = await actions.auth.getUserInfo()
    const isAdmin = user.role === "admin"

    return (
        <aside className="bg-white row-start-2  col-start-1">
            <nav className="space-y-8 px-5 py-2 flex flex-col justify-center h-8/12">
                <div>
                    <p className="text-gray-500 font-semibold text-sm">General</p>
                    <ul className="pt-2">
                        <li className="hover:bg-gray-200 p-2 rounded-md">
                            <Link className="flex gap-2 text-gray-700 hover:text-primary" href={"/dashboard"} >
                                <HomeIcon />
                                <p className="text-gray-700">Dashboard</p>
                            </Link>
                        </li>
                    </ul>
                </div>
                <div>
                    <p className="text-gray-500 font-semibold text-sm">Shop</p>
                    <ul className="pt-2">
                        <li className="hover:bg-gray-200 p-2 rounded-md">
                            <Link className="flex gap-2 text-gray-700 hover:text-primary" href={"/products"} >
                                <ShoppingBag />
                                <p className="text-gray-700">Products</p>
                            </Link>
                        </li>
                        <li className="hover:bg-gray-200 p-2 rounded-md">
                            <Link className="flex gap-2 text-gray-700 hover:text-primary" href={"/categories"} >
                                <StoreIcon />
                                <p className="text-gray-700">Categories</p>
                            </Link>
                        </li>
                        {isAdmin && (
                            <li className="hover:bg-gray-200 p-2 rounded-md">
                                <Link className="flex gap-2 text-gray-700 hover:text-primary" href={"/users"} >
                                    <Users />
                                    <p className="text-gray-700">Users</p>
                                </Link>
                            </li>
                        )}
                    </ul>
                </div>
                <div>
                    <p className="text-gray-500 font-semibold text-sm">Support</p>
                    <ul className="pt-2">
                        <li className="hover:bg-gray-200 p-2 rounded-md">
                            <Link className="flex gap-2 text-gray-700 hover:text-primary" href={"/dashboard"} >
                                <Settings />
                                <p className="text-gray-700">Settings</p>
                            </Link>
                        </li>
                    </ul>
                </div>
            </nav>
        </aside>
    )
}