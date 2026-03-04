import { NavLink, useNavigate } from 'react-router-dom';
import { FolderOpen, LogOut } from 'lucide-react';
import { useAuthStore } from '@/store/authStore';

function parseEmail(token: string): string {
  try {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/'))).email ?? '';
  } catch {
    return '';
  }
}

export default function Sidebar() {
  const navigate = useNavigate();
  const { accessToken, clearTokens } = useAuthStore();
  const email = accessToken ? parseEmail(accessToken) : '';

  const handleLogout = () => {
    clearTokens();
    navigate('/login');
  };

  return (
    <aside className="w-64 bg-white border-r border-gray-200 flex flex-col">
      <div className="p-4 border-b border-gray-200">
        <h1 className="text-lg font-semibold text-gray-900">TaskManager</h1>
        <p className="text-xs text-gray-500 mt-0.5 truncate">
          {email}
        </p>
      </div>

      <nav className="flex-1 p-3 space-y-1">
        <NavLink
          to="/projects"
          className={({ isActive }) =>
            `flex items-center gap-2 px-3 py-2 rounded-md text-sm transition-colors ${
              isActive
                ? 'bg-gray-100 text-gray-900 font-medium'
                : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
            }`
          }
        >
          <FolderOpen className="h-4 w-4 shrink-0" />
          Projects
        </NavLink>
      </nav>

      <div className="p-3 border-t border-gray-200">
        <button
          onClick={handleLogout}
          className="flex items-center gap-2 px-3 py-2 rounded-md text-sm text-gray-600 hover:bg-gray-50 hover:text-gray-900 w-full transition-colors"
        >
          <LogOut className="h-4 w-4 shrink-0" />
          Sign out
        </button>
      </div>
    </aside>
  );
}
