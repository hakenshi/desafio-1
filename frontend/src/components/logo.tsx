"use client";

import Image from "next/image";
import { useTheme } from "next-themes";
import { useEffect, useState } from "react";

interface LogoProps {
  width?: number;
  height?: number;
  className?: string;
}

export function Logo({ width = 100, height = 10, className }: LogoProps) {
  const { resolvedTheme } = useTheme();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  // Prevent hydration mismatch by showing light logo initially
  if (!mounted) {
    return (
      <Image
        width={width}
        height={height}
        priority
        src="/logo.svg"
        alt="hypesoft logo"
        className={className}
      />
    );
  }

  const logoSrc = resolvedTheme === "dark" ? "/logo-dark.svg" : "/logo.svg";

  return (
    <Image
      width={width}
      height={height}
      priority
      src={logoSrc}
      alt="hypesoft logo"
      className={className}
    />
  );
}
