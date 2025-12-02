"use client"

import { ColumnDef } from "@tanstack/react-table"
import { MoreHorizontal, ArrowUpDown } from "lucide-react"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { AuthModel } from "@/server/models/auth.model"
import { useState } from "react"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import UserCard from "@/components/user-card"
import UserForm from "@/components/forms/user-form"
import { actions } from "@/server/controllers"
import { useRouter } from "next/navigation"
import { Spinner } from "@/components/ui/spinner"

export type User = AuthModel.KeycloakUser

export const columns: ColumnDef<User>[] = [
  {
    accessorKey: "username",
    header: ({ column }) => {
      return (
        <Button
          variant="ghost"
          onClick={() => column.toggleSorting(column.getIsSorted() === "asc")}
        >
          Username
          <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
      )
    },
  },
  {
    accessorKey: "email",
    header: "Email",
  },
  {
    id: "fullName",
    header: "Full Name",
    cell: ({ row }) => {
      const firstName = row.original.firstName || ""
      const lastName = row.original.lastName || ""
      const fullName = `${firstName} ${lastName}`.trim()
      return fullName || "-"
    },
  },
  {
    accessorKey: "role",
    header: "Role",
    filterFn: (row, id, filterValue) => {
      return row.getValue(id) === filterValue
    },
    cell: ({ row }) => {
      const role = row.getValue("role") as string
      const roleColors: Record<string, string> = {
        admin: "bg-red-100 text-red-800",
        manager: "bg-blue-100 text-blue-800",
        user: "bg-gray-100 text-gray-800",
      }
      return (
        <span
          className={`capitalize px-2 py-1 rounded-md text-xs ${roleColors[role] || "bg-secondary text-secondary-foreground"}`}
        >
          {role}
        </span>
      )
    },
  },
  {
    accessorKey: "enabled",
    header: "Status",
    cell: ({ row }) => {
      const enabled = row.getValue("enabled") as boolean
      return (
        <span
          className={`px-2 py-1 rounded-md text-xs ${enabled ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}
        >
          {enabled ? "Active" : "Disabled"}
        </span>
      )
    },
  },
  {
    id: "actions",
    cell: ({ row }) => {
      const user = row.original
      const router = useRouter()

      const [showUserDetails, setShowUserDetails] = useState(false)
      const [showUpdateDialog, setShowUpdateDialog] = useState(false)
      const [showDeleteDialog, setShowDeleteDialog] = useState(false)
      const [isDeleting, setIsDeleting] = useState(false)

      const handleDelete = async () => {
        setIsDeleting(true)
        try {
          await actions.auth.deleteUser(user.id)
          router.refresh()
          setShowDeleteDialog(false)
        } catch (error) {
          console.error("Failed to delete user:", error)
        } finally {
          setIsDeleting(false)
        }
      }

      return (
        <>
          <DropdownMenu modal={false}>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" className="h-8 w-8 p-0">
                <span className="sr-only">Open menu</span>
                <MoreHorizontal className="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end">
              <DropdownMenuLabel>Actions</DropdownMenuLabel>
              <DropdownMenuItem
                onClick={() => navigator.clipboard.writeText(user.id)}
              >
                Copy user ID
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem onSelect={() => setShowUserDetails(true)}>
                Show Details
              </DropdownMenuItem>
              <DropdownMenuItem onSelect={() => setShowUpdateDialog(true)}>
                Edit user
              </DropdownMenuItem>
              <DropdownMenuItem onSelect={() => setShowDeleteDialog(true)} className="text-destructive">
                Delete user
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
          <Dialog open={showUserDetails} onOpenChange={setShowUserDetails}>
            <DialogContent>
              <DialogHeader className="sr-only">
                <DialogTitle>User Card</DialogTitle>
              </DialogHeader>
              <UserCard user={user} />
            </DialogContent>
          </Dialog>
          <Dialog open={showUpdateDialog} onOpenChange={setShowUpdateDialog}>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Edit User</DialogTitle>
              </DialogHeader>
              <UserForm user={user} onSuccess={() => setShowUpdateDialog(false)} />
            </DialogContent>
          </Dialog>
          <Dialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Delete User</DialogTitle>
                <DialogDescription>
                  Are you sure you want to delete user &quot;{user.username}&quot;? This action cannot be undone.
                </DialogDescription>
              </DialogHeader>
              <DialogFooter>
                <Button variant="outline" onClick={() => setShowDeleteDialog(false)} disabled={isDeleting}>
                  Cancel
                </Button>
                <Button variant="destructive" onClick={handleDelete} disabled={isDeleting}>
                  {isDeleting ? <><Spinner /> Deleting...</> : "Delete"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </>
      )
    },
  },
]
