"use client";

import { useTheme } from "next-themes";
import { Button } from "@/components/ui/button";
import { Moon, Sun, Monitor } from "lucide-react";
import { useEffect, useState } from "react";

export default function ThemeSettings() {
  const { theme, setTheme } = useTheme();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  if (!mounted) {
    return (
      <div className="grid grid-cols-3 gap-4">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-24 rounded-lg bg-muted animate-pulse" />
        ))}
      </div>
    );
  }

  const themes = [
    {
      value: "light",
      label: "Light",
      icon: Sun,
      description: "Light theme",
    },
    {
      value: "dark",
      label: "Dark",
      icon: Moon,
      description: "Dark theme",
    },
    {
      value: "system",
      label: "System",
      icon: Monitor,
      description: "Follow system",
    },
  ];

  return (
    <div className="grid grid-cols-3 gap-4">
      {themes.map(({ value, label, icon: Icon, description }) => (
        <Button
          key={value}
          variant={theme === value ? "default" : "outline"}
          className="h-24 flex-col gap-2"
          onClick={() => setTheme(value)}
        >
          <Icon className="h-6 w-6" />
          <span className="text-sm font-medium">{label}</span>
          <span className="text-xs text-muted-foreground">{description}</span>
        </Button>
      ))}
    </div>
  );
}
