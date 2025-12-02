import { AuthModel } from "@/server/models/auth.model"
import Icon from "./icon"

interface Props {
    user: AuthModel.UserInfo | AuthModel.KeycloakUser
}

function getRoleBadgeClass(role: string) {
    switch (role) {
        case "admin":
            return "bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400"
        case "manager":
            return "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400"
        default:
            return "bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-300"
    }
}

export default function UserCard({ user }: Props) {
    const fullName = [user.firstName, user.lastName].filter(Boolean).join(" ") || user.username

    return (
        <div>
            <div className="flex gap-4">
                <Icon size="lg" />
                <div className="flex flex-col justify-center">
                    <h3 className="text-lg font-semibold text-foreground">{fullName}</h3>
                    <p className="text-sm text-muted-foreground">@{user.username}</p>
                </div>
            </div>
            
            <div className="mt-4 pt-4 border-t border-border space-y-2">
                <div className="flex justify-between items-center">
                    <span className="text-sm text-muted-foreground">Email</span>
                    <span className="text-sm text-foreground">{user.email}</span>
                </div>
                <div className="flex justify-between items-center">
                    <span className="text-sm text-muted-foreground">Role</span>
                    <span className={`px-2 py-1 rounded-md text-xs font-medium capitalize ${getRoleBadgeClass(user.role)}`}>
                        {user.role}
                    </span>
                </div>
            </div>
        </div>
    )
}
