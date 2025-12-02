"use client";

import { AuthModel } from "@/server/models/auth.model";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
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
import { actions } from "@/server/controllers";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { toast } from "sonner";

const SettingsFormSchema = z.object({
  email: z.string().email("Invalid email address"),
  firstName: z.string().optional(),
  lastName: z.string().optional(),
});

type SettingsFormValues = z.infer<typeof SettingsFormSchema>;

interface Props {
  user: AuthModel.UserInfo;
}

export default function SettingsForm({ user }: Props) {
  const router = useRouter();
  const [isSuccess, setIsSuccess] = useState(false);

  const form = useForm<SettingsFormValues>({
    resolver: zodResolver(SettingsFormSchema),
    defaultValues: {
      email: user.email || "",
      firstName: user.firstName || "",
      lastName: user.lastName || "",
    },
  });

  const submit = async (values: SettingsFormValues) => {
    try {
      await actions.auth.updateUser(user.id, {
        email: values.email,
        firstName: values.firstName,
        lastName: values.lastName,
        role: user.role, // Keep the same role
      });
      setIsSuccess(true);
      router.refresh();
      toast.success("Profile updated successfully");
      setTimeout(() => setIsSuccess(false), 2000);
    } catch (error) {
      toast.error("Failed to update profile");
      console.error("Failed to update profile:", error);
    }
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(submit)} className="space-y-4">
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

        <Button
          disabled={form.formState.isSubmitting || isSuccess}
          type="submit"
          className="w-full"
        >
          {form.formState.isSubmitting ? (
            <>
              <Spinner /> Saving...
            </>
          ) : isSuccess ? (
            "Saved!"
          ) : (
            "Save Changes"
          )}
        </Button>
      </form>
    </Form>
  );
}
