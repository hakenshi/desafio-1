import Image from "next/image";
import { Input } from "./ui/input";
import { BellIcon, EllipsisIcon, SearchIcon, SunIcon } from "lucide-react";
import Icon from "./icon";
import { actions } from "@/server/controllers";
import Link from "next/link";

export async function Header() {

    const user = await actions.auth.getUserInfo()

    return (
        <header className="w-full bg-white col-span-2 row-span-1">
            <nav className="flex justify-between items-center px-5 py-3 mx-auto">
                <Link href={"/dashboard"} className="text-center flex">
                    <Image
                        width={100}
                        height={10}
                        priority
                        src="/logo.svg"
                        alt="hypesoft logo"
                    />
                </Link>
                <div className="relative w-1/4">
                    <SearchIcon className="absolute top-1/2 -translate-y-1/2 left-3 text-gray-400" size={18} />
                    <Input placeholder="Search" className="pl-10 bg-gray-100 rounded-full border-none w-full" />
                </div>
                <div className="flex gap-2 items-center">
                    <div className="flex gap-2 border-r border-gray-300 px-2">
                        <SunIcon stroke="#6a7282" fill="#6a7282" />
                        <BellIcon stroke="#6a7282" fill="#6a7282" />
                    </div>
                    <div className="flex items-center gap-3">
                        <Icon />
                        <div className="text-start">
                            <p className="text-gray-800 text-sm">{user.firstName}</p>
                            <p className="text-xs text-gray-500">{user.role}</p>
                        </div>
                        <EllipsisIcon className="ml-5 text-gray-600" />
                    </div>
                </div>
            </nav>
        </header>
    )
}