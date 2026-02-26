
import { Link } from "react-router-dom";

const baseStyles =
  "inline-flex items-center justify-center rounded-md font-medium transition focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-black disabled:opacity-50 disabled:pointer-events-none hover:bg-white hover:text-black";

const sizeStyles = {
  sm: "text-sm px-3 py-1.5",
  md: "text-base px-4 py-2",
  lg: "text-lg px-6 py-3",
};

const variantStyles = {
  primary: "bg-black text-white hover:bg-gray-800",
  outline:
    "border border-white text-white hover:bg-white hover:text-black",
};

function Button({
  children,
  onClick,
  type = "button",
  disabled = false,
  to,
  size = "md",
  variant = "primary",
  className = "",
}) {
  const classes = [
    baseStyles,
    sizeStyles[size] || sizeStyles.md,
    variantStyles[variant] || variantStyles.primary,
    disabled ? "pointer-events-none opacity-50" : "",
    className,
  ]
    .filter(Boolean)
    .join(" ");

  if (to) {
    return (
      <Link
        to={to}
        onClick={disabled ? (event) => event.preventDefault() : onClick}
        aria-disabled={disabled}
        className={classes}
      >
        {children}
      </Link>
    );
  }

  return (
    <button
      type={type}
      onClick={onClick}
      disabled={disabled}
      className={classes}
    >
      {children}
    </button>
  );
}

export default Button;
export { Button };
