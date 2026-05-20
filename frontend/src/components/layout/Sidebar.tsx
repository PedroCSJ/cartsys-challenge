"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  Users,
  MapPin,
  Building2,
  Code2,
  LayoutDashboard,
  UserCircle,
} from "lucide-react";
import { cn } from "@/lib/utils";

const navItems = [
  { href: "/", label: "Dashboard", icon: LayoutDashboard },
  { href: "/developers", label: "Desenvolvedores", icon: Code2 },
  { href: "/languages", label: "Linguagens", icon: UserCircle },
  { href: "/cities", label: "Cidades", icon: Building2 },
  { href: "/states", label: "Estados", icon: MapPin },
  { href: "/users", label: "Usuários", icon: Users },
];

export function Sidebar() {
  const pathname = usePathname();

  return (
    <aside className="w-60 shrink-0 border-r bg-card h-full flex flex-col">
      <div className="p-6 border-b">
        <h1 className="text-xl font-bold text-primary">Cartsys</h1>
        <p className="text-xs text-muted-foreground mt-0.5">
          Gestão de Desenvolvedores
        </p>
      </div>

      <nav className="flex-1 p-3 space-y-1">
        {navItems.map(({ href, label, icon: Icon }) => {
          const isActive =
            href === "/" ? pathname === "/" : pathname.startsWith(href);

          return (
            <Link
              key={href}
              href={href}
              className={cn(
                "flex items-center gap-3 px-3 py-2 rounded-md text-sm font-medium transition-colors",
                isActive
                  ? "bg-primary text-primary-foreground"
                  : "text-muted-foreground hover:bg-muted hover:text-foreground"
              )}
            >
              <Icon size={16} />
              {label}
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}
