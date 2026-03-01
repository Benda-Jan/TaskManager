import { Routes, Route, Navigate } from 'react-router-dom';
import AppLayout from './components/layout/AppLayout';
import ProjectsPage from './pages/ProjectsPage';
import ProjectBoardPage from './pages/ProjectBoardPage';
import ExpensesPage from './pages/ExpensesPage';
import ProjectSettingsPage from './pages/ProjectSettingsPage';

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<AppLayout />}>
        <Route index element={<Navigate to="/projects" replace />} />
        <Route path="projects" element={<ProjectsPage />} />
        <Route path="projects/:id" element={<ProjectBoardPage />} />
        <Route path="projects/:id/expenses" element={<ExpensesPage />} />
        <Route path="projects/:id/settings" element={<ProjectSettingsPage />} />
      </Route>
    </Routes>
  );
}
