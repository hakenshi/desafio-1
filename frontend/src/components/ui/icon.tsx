import { User2Icon, UserIcon } from "lucide-react"
import { Avatar, AvatarFallback, AvatarImage } from "./avatar"

interface Props {
    src?: string
}

export default function Icon({src}: Props){
    return (
        <Avatar>
            <AvatarImage src={src} />
            <AvatarFallback>
                <div className="bg-gray-300 p-2 rounded-full">
                    <User2Icon className="text-gray-500" />
                </div>
            </AvatarFallback>
        </Avatar>
    )
}