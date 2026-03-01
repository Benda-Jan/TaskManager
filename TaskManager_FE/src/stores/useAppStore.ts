import { create } from 'zustand';

interface AppState {
  selectedProjectId: string | null;
  setSelectedProjectId: (id: string | null) => void;
}

const useAppStore = create<AppState>((set) => ({
  selectedProjectId: null,
  setSelectedProjectId: (id) => set({ selectedProjectId: id }),
}));

export default useAppStore;
