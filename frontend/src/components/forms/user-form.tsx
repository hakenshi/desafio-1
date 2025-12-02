"use client";

import { AuthModel } from "@/server/models/auth.model";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import {
    Form,
    FormControl,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from "../ui/form";
import { Input } from "../ui/input";
import { Button } from "../ui/button";
import { Spinner } from "../ui/spinner";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "../ui/select";
import { actions } from "@/server/controllers";
import { useRouter } from "next/navigation";

interface Props {
    user?: AuthModel.KeycloakUser;
    onSuccess?: () => void;
}

export default function UserForm({ user, onSuccess }: Props) {
    const isUpdating = !!user;
    const router = useRouter();

    const form = useForm<AuthModel.RegisterRequest>({
        resolver: zodResolver(AuthModel.RegisterRequestSchema),
        defaultValues: {
            username: user?.username || "",
            email: user?.email || "",
            firstName: user?.firstName || "",
            lastName: user?.lastName || "",
            password: "",
            role: user?.role || "user",
        }
    });

    const submit = async (values: AuthModel.RegisterRequest) => {
        if (isUpdating) {
            await actions.auth.updateUser(user.id, {
                email: values.email,
                firstName: values.firstName,
                lastName: values.lastName,
                role: values.role,
            });
        } else {
            await actions.auth.register(values);
        }
        router.refresh();
        onSuccess?.();
    };

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(submit)} className="space-y-4">
                <FormField
                    control={form.control}
                    name="username"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Username</FormLabel>
                            <FormControl>
                                <Input
                                    placeholder="Username"
                                    {...field}
                                />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <FormField
                    control={form.control}
                    name="email"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Email</FormLabel>
                            <FormControl>
                                <Input
                                    type="email"
                                    placeholder="email@example.com"
                                    {...field}
                                />
                            </FormControl>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <div className="grid grid-cols-2 gap-4">
                    <FormField
                        control={form.control}
                        name="firstName"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>First Name</FormLabel>
                                <FormControl>
                                    <Input placeholder="First name" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />

                    <FormField
                        control={form.control}
                        name="lastName"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Last Name</FormLabel>
                                <FormControl>
                                    <Input placeholder="Last name" {...field} />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                </div>

                {!isUpdating && (
                    <FormField
                        control={form.control}
                        name="password"
                        render={({ field }) => (
                            <FormItem>
                                <FormLabel>Password</FormLabel>
                                <FormControl>
                                    <Input
                                        type="password"
                                        placeholder="••••••••"
                                        {...field}
                                    />
                                </FormControl>
                                <FormMessage />
                            </FormItem>
                        )}
                    />
                )}

                <FormField
                    control={form.control}
                    name="role"
                    render={({ field }) => (
                        <FormItem>
                            <FormLabel>Role</FormLabel>
                            <Select onValueChange={field.onChange} defaultValue={field.value}>
                                <FormControl>
                                    <SelectTrigger className="w-full">
                                        <SelectValue placeholder="Select a role" />
                                    </SelectTrigger>
                                </FormControl>
                                <SelectContent>
                                    <SelectItem value="user">User</SelectItem>
                                    <SelectItem value="manager">Manager</SelectItem>
                                    <SelectItem value="admin">Admin</SelectItem>
                                </SelectContent>
                            </Select>
                            <FormMessage />
                        </FormItem>
                    )}
                />

                <Button
                    disabled={form.formState.isSubmitting}
                    type="submit"
                    className="w-full"
                >
                    {form.formState.isSubmitting ? (
                        <>
                            <Spinner /> {isUpdating ? "Updating..." : "Creating..."}
                        </>
                    ) : isUpdating ? (
                        "Update User"
                    ) : (
                        "Create User"
                    )}
                </Button>
            </form>
        </Form>
    );
}
