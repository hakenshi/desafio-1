"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { AlertTriangle, Clock, RefreshCw } from "lucide-react";

interface ErrorProps {
  error: Error & { digest?: string };
  reset: () => void;
}

export default function Error({ error, reset }: ErrorProps) {
  const [countdown, setCountdown] = useState(0);
  const [isRateLimited, setIsRateLimited] = useState(false);

  useEffect(() => {
    // Check if it's a rate limit error (429)
    const is429 = error.message?.includes("429") || 
                  error.message?.toLowerCase().includes("too many requests") ||
                  error.message?.toLowerCase().includes("rate limit");
    
    setIsRateLimited(is429);
    
    if (is429) {
      setCountdown(60); // 60 seconds cooldown
    }
  }, [error]);

  useEffect(() => {
    if (countdown <= 0) return;

    const timer = setInterval(() => {
      setCountdown((prev) => {
        if (prev <= 1) {
          clearInterval(timer);
          return 0;
        }
        return prev - 1;
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [countdown]);

  const handleRetry = () => {
    if (isRateLimited && countdown > 0) return;
    reset();
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-4">
      <div className="max-w-md w-full text-center space-y-6">
        <div className="flex justify-center">
          {isRateLimited ? (
            <div className="p-4 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <Clock className="h-12 w-12 text-orange-600 dark:text-orange-400" />
            </div>
          ) : (
            <div className="p-4 rounded-full bg-destructive/10">
              <AlertTriangle className="h-12 w-12 text-destructive" />
            </div>
          )}
        </div>

        <div className="space-y-2">
          <h1 className="text-2xl font-bold">
            {isRateLimited ? "Muitas requisições" : "Algo deu errado"}
          </h1>
          <p className="text-muted-foreground">
            {isRateLimited
              ? "Você fez muitas requisições em pouco tempo. Aguarde um momento antes de tentar novamente."
              : "Ocorreu um erro inesperado. Por favor, tente novamente."}
          </p>
        </div>

        {isRateLimited && countdown > 0 && (
          <div className="space-y-3">
            <div className="text-5xl font-mono font-bold text-orange-600 dark:text-orange-400">
              {countdown}s
            </div>
            <div className="w-full bg-muted rounded-full h-2 overflow-hidden">
              <div
                className="h-full bg-orange-600 dark:bg-orange-400 transition-all duration-1000 ease-linear"
                style={{ width: `${(countdown / 60) * 100}%` }}
              />
            </div>
          </div>
        )}

        <Button
          onClick={handleRetry}
          disabled={isRateLimited && countdown > 0}
          className="gap-2"
        >
          <RefreshCw className="h-4 w-4" />
          {isRateLimited && countdown > 0 ? "Aguarde..." : "Tentar novamente"}
        </Button>

        {process.env.NODE_ENV === "development" && (
          <details className="text-left text-xs text-muted-foreground mt-4">
            <summary className="cursor-pointer hover:text-foreground">
              Detalhes do erro (dev only)
            </summary>
            <pre className="mt-2 p-3 bg-muted rounded-md overflow-auto max-h-40">
              {error.message}
              {error.digest && `\nDigest: ${error.digest}`}
            </pre>
          </details>
        )}
      </div>
    </div>
  );
}
