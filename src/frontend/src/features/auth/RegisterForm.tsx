"use client";

import { memo } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useRegister } from "@/hooks/use-register";

type RegisterFormProps = {
  onClose?: () => void;
  onSwitchToLogin?: () => void;
};

function RegisterFormComponent({ onClose, onSwitchToLogin }: RegisterFormProps) {
  const form = useRegister({ onSuccess: onClose });
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = form;

  return (
    <div className="w-full max-w-lg rounded-card border border-fabu-border bg-white p-6 shadow-modal">
      <div className="mb-6 flex items-start justify-between gap-4">
        <div>
          <h1 className="text-3xl">Create Account</h1>
          <p className="mt-2 text-sm text-fabu-gray">
            Register with the fields required by the Fabu backend.
          </p>
        </div>
        {onClose ? (
          <button
            type="button"
            className="flex h-11 w-11 items-center justify-center rounded-full bg-fabu-muted text-xl text-fabu-charcoal hover:bg-[#E7E7E7]"
            onClick={onClose}
            aria-label="Close register"
          >
            x
          </button>
        ) : null}
      </div>

      <form className="grid gap-4 md:grid-cols-2" onSubmit={handleSubmit(form.onRegisterSubmit)}>
        <div className="space-y-1.5">
          <label className="fabu-label" htmlFor="register-username">
            Username
          </label>
          <Input id="register-username" placeholder="fabu_user" {...register("username")} />
          {errors.username ? <p className="fabu-error">{errors.username.message}</p> : null}
        </div>

        <div className="space-y-1.5">
          <label className="fabu-label" htmlFor="register-full-name">
            Full name
          </label>
          <Input id="register-full-name" placeholder="Nguyen Van A" {...register("fullName")} />
          {errors.fullName ? <p className="fabu-error">{errors.fullName.message}</p> : null}
        </div>

        <div className="space-y-1.5">
          <label className="fabu-label" htmlFor="register-email">
            Email
          </label>
          <Input id="register-email" type="email" placeholder="you@fabu.vn" {...register("email")} />
          {errors.email ? <p className="fabu-error">{errors.email.message}</p> : null}
        </div>

        <div className="space-y-1.5">
          <label className="fabu-label" htmlFor="register-phone">
            Phone number
          </label>
          <Input id="register-phone" placeholder="0912345678" {...register("phoneNumber")} />
          {errors.phoneNumber ? <p className="fabu-error">{errors.phoneNumber.message}</p> : null}
        </div>

        <div className="space-y-1.5">
          <label className="fabu-label" htmlFor="register-password">
            Password
          </label>
          <Input
            id="register-password"
            type="password"
            placeholder="At least 6 characters"
            {...register("password")}
          />
          {errors.password ? <p className="fabu-error">{errors.password.message}</p> : null}
        </div>

        <div className="space-y-1.5">
          <label className="fabu-label" htmlFor="register-confirm-password">
            Confirm password
          </label>
          <Input
            id="register-confirm-password"
            type="password"
            placeholder="Repeat password"
            {...register("confirmPassword")}
          />
          {errors.confirmPassword ? (
            <p className="fabu-error">{errors.confirmPassword.message}</p>
          ) : null}
        </div>

        <div className="md:col-span-2">
          <Button type="submit" className="w-full" disabled={isSubmitting}>
            {isSubmitting ? "Creating..." : "Create account"}
          </Button>
        </div>

        {onSwitchToLogin ? (
          <button
            type="button"
            className="text-sm font-semibold text-fabu-red hover:text-fabu-red-hover md:col-span-2"
            onClick={onSwitchToLogin}
          >
            Already have an account? Sign in
          </button>
        ) : null}
      </form>
    </div>
  );
}

export const RegisterForm = memo(RegisterFormComponent);
