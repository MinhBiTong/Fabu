"use client";

import { memo } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useLogin } from "@/hooks/use-login";

type LoginFormProps = {
  onClose?: () => void;
  onSwitchToSignup?: () => void;
};

function LoginFormComponent({ onClose, onSwitchToSignup }: LoginFormProps) {
  const form = useLogin({ onSuccess: onClose });
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = form;

  return (
    <div className="w-full max-w-md rounded-card border border-fabu-border bg-white p-6 shadow-modal">
      <div className="mb-6 flex items-start justify-between gap-4">
        <div>
          <h1 className="text-3xl">Sign In</h1>
          <p className="mt-2 text-sm text-fabu-gray">
            Access your Fabu account and manage telecom services.
          </p>
        </div>
        {onClose ? (
          <button
            type="button"
            className="flex h-11 w-11 items-center justify-center rounded-full bg-fabu-muted text-xl text-fabu-charcoal hover:bg-[#E7E7E7]"
            onClick={onClose}
            aria-label="Close login"
          >
            x
          </button>
        ) : null}
      </div>

      <form className="space-y-4" onSubmit={handleSubmit(form.onLoginSubmit)}>
        <div className="space-y-1.5">
          <label className="fabu-label" htmlFor="login-email">
            Email
          </label>
          <Input id="login-email" type="email" placeholder="you@fabu.vn" {...register("email")} />
          {errors.email ? <p className="fabu-error">{errors.email.message}</p> : null}
        </div>

        <div className="space-y-1.5">
          <label className="fabu-label" htmlFor="login-password">
            Password
          </label>
          <Input
            id="login-password"
            type="password"
            placeholder="Enter password"
            {...register("password")}
          />
          {errors.password ? <p className="fabu-error">{errors.password.message}</p> : null}
        </div>

        <div className="flex items-center justify-between gap-3 text-sm">
          <button type="button" className="text-fabu-charcoal hover:text-fabu-red">
            Forgot password
          </button>
          {onSwitchToSignup ? (
            <button
              type="button"
              className="font-semibold text-fabu-red hover:text-fabu-red-hover"
              onClick={onSwitchToSignup}
            >
              Create account
            </button>
          ) : null}
        </div>

        <Button type="submit" className="w-full" disabled={isSubmitting}>
          {isSubmitting ? "Signing in..." : "Sign in"}
        </Button>

        <Button
          type="button"
          variant="outline"
          className="w-full"
          onClick={form.handleGoogleLogin}
        >
          Sign in with Google
        </Button>
      </form>
    </div>
  );
}

export const LoginForm = memo(LoginFormComponent);
