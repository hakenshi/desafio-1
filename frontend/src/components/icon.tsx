import { User2Icon, UserCircle2Icon, UserCircleIcon, UserIcon } from "lucide-react"
import { Avatar, AvatarFallback, AvatarImage } from "./ui/avatar"

interface Props {
    src?: string
}

export default function Icon({src}: Props){
    return (
        <Avatar>
            <AvatarImage src={src} />
            <AvatarFallback>
                <div className="bg-gray-300 p-2 rounded-full">
                    <User2Icon strokeWidth={0} fill="#6a7282" className="text-gray-500" />
                </div>
            </AvatarFallback>
        </Avatar>
    )
}