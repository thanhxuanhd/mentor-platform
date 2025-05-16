import { z } from "zod";

const passwordSchema = z
  .string()
  .min(1, "Password must not be blank")
  .min(8, { message: "Password must be at least 8 characters long" })
  .max(50, { message: "Password must be less than 50 characters" });

export const loginSchema = z.object({
  username: z
    .string()
    .min(1, "Username must not be blank")
    .min(2, { message: "Username must be at least 2 characters" })
    .max(50, { message: "Username must be less than 50 characters" }),
  password: passwordSchema,
});