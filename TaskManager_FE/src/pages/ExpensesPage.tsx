import { useParams } from 'react-router-dom';

export default function ExpensesPage() {
  const { id } = useParams<{ id: string }>();

  return (
    <div className="p-6">
      <h2 className="text-2xl font-semibold text-gray-900 mb-6">Expenses</h2>
      <p className="text-gray-500 text-sm">Project ID: {id}</p>
    </div>
  );
}
