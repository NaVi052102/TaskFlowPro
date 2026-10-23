import React, { useState, useEffect } from 'react'; // Import React library and state/effect hooks
import { useNavigate } from 'react-router-dom'; // Import router navigation hook
import api from '../services/api'; // Import configured Axios instance for backend API communication
import { LayoutDashboard, CheckSquare, Square, PlusCircle, LogOut, FolderPlus, Trash2, Edit3, X, Check, CheckCircle2, Clock, Loader2, Search, Calendar, ArrowUpDown, ChevronLeft, ChevronRight, AlertCircle, User, Settings, Save } from 'lucide-react'; // Import UI icons including User and Settings for profile modal
import toast from 'react-hot-toast'; // Import hot-toast notification library

export default function DashboardPage() { // Export default dashboard page functional component
    const navigate = useNavigate(); // Initialize navigate hook for page redirection

    const [projects, setProjects] = useState([]); // State hook for storing fetched projects array
    const [selectedProject, setSelectedProject] = useState(null); // State hook for currently selected project object
    const [tasks, setTasks] = useState([]); // State hook for storing tasks of selected project
    
    const [loadingProjects, setLoadingProjects] = useState(true); // State hook for project loading indicator
    const [loadingTasks, setLoadingTasks] = useState(false); // State hook for task loading indicator

    const [projectName, setProjectName] = useState(''); // State hook for new project name input
    const [projectDesc, setProjectDesc] = useState(''); // State hook for new project description input
    const [projectSearch, setProjectSearch] = useState(''); // State hook for project search filter input query

    const [taskTitle, setTaskTitle] = useState(''); // State hook for new task title input
    const [taskDesc, setTaskDesc] = useState(''); // State hook for new task description input
    const [taskPriority, setTaskPriority] = useState(1); // State hook for task priority selection
    const [taskDueDate, setTaskDueDate] = useState(''); // State hook for new task due date input
    const [taskFilter, setTaskFilter] = useState('all'); // State hook for task filter tab state ('all', 'active', 'completed', 'overdue')
    const [taskSearch, setTaskSearch] = useState(''); // State hook for task search input query
    const [sortBy, setSortBy] = useState('created'); // State hook for sorting tasks ('created', 'priority', 'dueDate')

    // --- Task Editing State Hooks ---
    const [editingTaskId, setEditingTaskId] = useState(null); // State hook for ID of task being edited
    const [editTaskTitle, setEditTaskTitle] = useState(''); // State hook for edited task title value
    const [editTaskDesc, setEditTaskDesc] = useState(''); // State hook for edited task description value
    const [editTaskPriority, setEditTaskPriority] = useState(1); // State hook for edited task priority value
    const [editTaskDueDate, setEditTaskDueDate] = useState(''); // State hook for edited task due date value

    // --- Profile Modal State Hooks ---
    const [isProfileOpen, setIsProfileOpen] = useState(false); // State hook for controlling profile modal visibility
    const [profileName, setProfileName] = useState('Ivan Quevedo'); // State hook for user display name
    const [profileEmail, setProfileEmail] = useState('ivan@example.com'); // State hook for user email address

    // --- Pagination State Hooks ---
    const [currentPage, setCurrentPage] = useState(1); // State hook for tracking current active pagination page
    const tasksPerPage = 5; // Constant defining maximum number of tasks displayed per page

    const [editingProjectId, setEditingProjectId] = useState(null); // State hook for ID of project being edited
    const [editProjectName, setEditProjectName] = useState(''); // State hook for edited project name value
    const [editProjectDesc, setEditProjectDesc] = useState(''); // State hook for edited project description value

    const [error, setError] = useState(''); // State hook for error notification messages

    const fetchProjects = async () => { // Asynchronous function to fetch user projects from backend
        try { // Begin try block for error handling
            setLoadingProjects(true); // Set project loading state to true
            const response = await api.get('/projects'); // Send GET request to projects API endpoint
            setProjects(response.data); // Update projects state with response data
        } catch (err) { // Catch block for handling API failures
            console.error('Failed to fetch projects', err); // Log error to console
            setError('Failed to load projects from server.'); // Set error message state
            toast.error('Failed to load projects.'); // Trigger error toast notification
        } finally { // Finally block executing after try/catch
            setLoadingProjects(false); // Set project loading state to false
        } // End try-catch-finally block
    }; // End fetchProjects function

    const fetchTasks = async (projectId) => { // Asynchronous function to fetch tasks for a given project ID
        try { // Begin try block
            setLoadingTasks(true); // Set task loading state to true
            const response = await api.get('/tasks?projectId=' + projectId); // Send GET request filtering tasks by project ID using concatenation
            setTasks(response.data); // Update tasks state with response data
            setCurrentPage(1); // Reset pagination back to page 1 upon fetching new tasks
        } catch (err) { // Catch block for errors
            console.error('Failed to fetch tasks', err); // Log error
            setError('Failed to load tasks for this project.'); // Set error state
            toast.error('Failed to load tasks.'); // Trigger error toast notification
        } finally { // Finally block
            setLoadingTasks(false); // Set task loading state to false
        } // End try-catch-finally
    }; // End fetchTasks function

    const handleSelectProject = (project) => { // Function handler when a project card is clicked
        setSelectedProject(project); // Set selected project state object
        setTaskSearch(''); // Reset task search query when switching projects
        fetchTasks(project.id); // Call fetchTasks passing the project identifier
    }; // End handleSelectProject

    const handleCreateProject = async (e) => { // Asynchronous function handler for creating a project
        e.preventDefault(); // Prevent default browser form reload behavior
        const toastId = toast.loading('Creating project...'); // Show loading toast while request processes
        try { // Begin try block
            await api.post('/projects', { name: projectName, description: projectDesc }); // Send POST request to create project
            setProjectName(''); // Reset project name input field
            setProjectDesc(''); // Reset project description input field
            fetchProjects(); // Refresh projects list from server
            toast.success('Project created successfully!', { id: toastId }); // Update loading toast to success
        } catch (err) { // Catch block
            setError('Failed to create project.'); // Set error message state
            toast.error('Failed to create project.', { id: toastId }); // Update loading toast to error
        } // End try-catch
    }; // End handleCreateProject

    const handleStartEditProject = (project, e) => { // Function to initialize project inline editing mode
        e.stopPropagation(); // Stop click event propagation to parent elements
        setEditingProjectId(project.id); // Set editing project ID state
        setEditProjectName(project.name); // Pre-fill edit name input with current name
        setEditProjectDesc(project.description); // Pre-fill edit description input with current description
    }; // End handleStartEditProject

    const handleCancelEditProject = (e) => { // Function to cancel project editing mode
        e.stopPropagation(); // Stop click propagation
        setEditingProjectId(null); // Clear editing project ID state
        setEditProjectName(''); // Clear edit name state
        setEditProjectDesc(''); // Clear edit description state
    }; // End handleCancelEditProject

    const handleUpdateProject = async (projectId, e) => { // Asynchronous function to save updated project data
        e.stopPropagation(); // Stop click propagation
        const toastId = toast.loading('Updating project...'); // Show loading toast
        try { // Begin try block
            await api.put('/projects/' + projectId, { // Send PUT request with updated project payload using concatenation
                name: editProjectName, // Send updated name
                description: editProjectDesc // Send updated description
            }); // End API PUT request
            setEditingProjectId(null); // Clear editing project ID state
            fetchProjects(); // Refresh projects list
            if (selectedProject && selectedProject.id === projectId) { // Check if edited project is currently active
                setSelectedProject({ ...selectedProject, name: editProjectName, description: editProjectDesc }); // Update selected project object state
            } // End conditional check
            toast.success('Project updated successfully!', { id: toastId }); // Trigger success toast
        } catch (err) { // Catch block
            console.error('Failed to update project', err); // Log error
            setError('Failed to update project.'); // Set error message state
            toast.error('Failed to update project.', { id: toastId }); // Trigger error toast
        } // End try-catch
    }; // End handleUpdateProject

    const handleDeleteProject = async (projectId, e) => { // Asynchronous function to delete a project
        e.stopPropagation(); // Stop click propagation
        if (!window.confirm('Are you sure you want to delete this project and all its tasks?')) return; // Confirm deletion with user
        const toastId = toast.loading('Deleting project...'); // Show loading toast
        try { // Begin try block
            await api.delete('/projects/' + projectId); // Send DELETE request using concatenation
            fetchProjects(); // Refresh projects list
            if (selectedProject && selectedProject.id === projectId) { // Check if deleted project was selected
                setSelectedProject(null); // Clear selected project state
                setTasks([]); // Clear tasks state array
            } // End conditional check
            toast.success('Project deleted successfully.', { id: toastId }); // Trigger success toast
        } catch (err) { // Catch block
            console.error('Failed to delete project', err); // Log error
            setError('Failed to delete project.'); // Set error message state
            toast.error('Failed to delete project.', { id: toastId }); // Trigger error toast
        } // End try-catch
    }; // End handleDeleteProject

    const handleCreateTask = async (e) => { // Asynchronous function to create a new task
        e.preventDefault(); // Prevent default form submission
        if (!selectedProject) return; // Guard clause if no project is active
        const toastId = toast.loading('Adding task...'); // Show loading toast
        try { // Begin try block
            await api.post('/tasks', { // Send POST request to create task
                title: taskTitle, // Task title payload
                description: taskDesc, // Task description payload
                projectId: selectedProject.id, // Associated project identifier
                priority: parseInt(taskPriority), // Task priority number parsed from state
                dueDate: taskDueDate ? taskDueDate : null, // Task due date ISO string payload or null
                isCompleted: false // Initialize new task as uncompleted
            }); // End API call
            setTaskTitle(''); // Reset task title input
            setTaskDesc(''); // Reset task description input
            setTaskPriority(1); // Reset priority back to Medium default
            setTaskDueDate(''); // Reset due date input
            setCurrentPage(1); // Reset pagination back to first page
            fetchTasks(selectedProject.id); // Refresh tasks list for active project
            toast.success('Task added successfully!', { id: toastId }); // Trigger success toast
        } catch (err) { // Catch block
            setError('Failed to create task.'); // Set error message state
            toast.error('Failed to add task.', { id: toastId }); // Trigger error toast
        } // End try-catch
    }; // End handleCreateTask

    const handleToggleTask = async (task) => { // Asynchronous function to toggle task completion status
        try { // Begin try block
            const updatedCompleted = !task.isCompleted; // Toggle completion boolean value
            await api.put('/tasks/' + task.id, { // Send PUT request to update task status matching backend UpdateTaskDto
                title: task.title, // Task title
                description: task.description, // Task description
                priority: task.priority, // Task priority
                dueDate: task.dueDate, // Keep existing due date
                isCompleted: updatedCompleted, // Send boolean expected by backend
                projectId: selectedProject.id // Project identifier
            }); // End API call
            fetchTasks(selectedProject.id); // Refresh tasks list from server
            if (updatedCompleted) {
                toast.success('Task marked as completed!'); // Trigger success toast on completion
            } else {
                toast('Task marked as active.', { icon: '🔄' }); // Trigger info toast when reopened
            }
        } catch (err) { // Catch block
            console.error('Failed to update task status', err); // Log error
            toast.error('Failed to update task status.'); // Trigger error toast
        } // End try-catch
    }; // End handleToggleTask

    // --- Task Editing Handlers ---
    const handleStartEditTask = (task) => { // Function to start editing a task
        setEditingTaskId(task.id); // Set task ID being edited
        setEditTaskTitle(task.title); // Set edit title state
        setEditTaskDesc(task.description); // Set edit description state
        setEditTaskPriority(task.priority ?? 1); // Set edit priority state
        setEditTaskDueDate(task.dueDate ? task.dueDate.split('T')[0] : ''); // Format due date for date input
    };

    const handleCancelEditTask = () => { // Function to cancel task editing
        setEditingTaskId(null); // Clear editing task ID
        setEditTaskTitle(''); // Clear edit title
        setEditTaskDesc(''); // Clear edit description
        setEditTaskPriority(1); // Reset priority
        setEditTaskDueDate(''); // Clear due date
    };

    const handleUpdateTask = async (taskId, originalTask) => { // Asynchronous function to save task edits
        const toastId = toast.loading('Updating task...'); // Show loading toast
        try { // Begin try block
            await api.put('/tasks/' + taskId, { // Send PUT request to update task details
                title: editTaskTitle, // Updated title
                description: editTaskDesc, // Updated description
                priority: parseInt(editTaskPriority), // Updated priority
                dueDate: editTaskDueDate ? editTaskDueDate : null, // Updated due date
                isCompleted: originalTask.isCompleted, // Retain original completion state
                projectId: selectedProject.id // Project identifier
            });
            setEditingTaskId(null); // Clear editing state
            fetchTasks(selectedProject.id); // Refresh tasks list
            toast.success('Task updated successfully!', { id: toastId }); // Success toast
        } catch (err) { // Catch block
            console.error('Failed to update task', err); // Log error
            toast.error('Failed to update task.', { id: toastId }); // Error toast
        }
    };

    const handleDeleteTask = async (taskId) => { // Asynchronous function to delete a task by ID
        const toastId = toast.loading('Deleting task...'); // Show loading toast
        try { // Begin try block
            await api.delete('/tasks/' + taskId); // Send DELETE request using concatenation
            fetchTasks(selectedProject.id); // Refresh tasks list
            toast.success('Task deleted successfully.', { id: toastId }); // Trigger success toast
        } catch (err) { // Catch block
            console.error('Failed to delete task', err); // Log error
            toast.error('Failed to delete task.', { id: toastId }); // Trigger error toast
        } // End try-catch
    }; // End handleDeleteTask

    // --- Profile Save Handler ---
    const handleSaveProfile = (e) => { // Function to save user profile changes
        e.preventDefault(); // Prevent page refresh
        toast.success('Profile updated successfully!'); // Trigger success toast
        setIsProfileOpen(false); // Close profile modal
    };

    const renderPriorityBadge = (priority) => { // Helper function to render visual priority badges
        switch (parseInt(priority)) { // Switch statement based on priority number value
            case 2: // High priority case
                return <span className="px-2 py-0.5 text-xs font-semibold bg-red-500/10 text-red-400 border border-red-500/30 rounded-full">High</span>; // Return High badge JSX
            case 1: // Medium priority case
                return <span className="px-2 py-0.5 text-xs font-semibold bg-amber-500/10 text-amber-400 border border-amber-500/30 rounded-full">Medium</span>; // Return Medium badge JSX
            case 0: // Low priority case
            default: // Default case
                return <span className="px-2 py-0.5 text-xs font-semibold bg-emerald-500/10 text-emerald-400 border border-emerald-500/30 rounded-full">Low</span>; // Return Low badge JSX
        } // End switch statement
    }; // End renderPriorityBadge

    // Filter tasks based on search query and status filter tab (including 'overdue')
    const filteredTasks = tasks.filter((task) => {
        const matchesSearch = task.title.toLowerCase().includes(taskSearch.toLowerCase()) || 
                              task.description.toLowerCase().includes(taskSearch.toLowerCase()); // Check search match
        if (!matchesSearch) return false; // Exclude task if search query does not match title or description

        const isCompleted = task.isCompleted; // Check completion boolean
        const isOverdue = task.dueDate && !isCompleted && new Date(task.dueDate) < new Date(new Date().toDateString()); // Check overdue condition

        if (taskFilter === 'active') return !isCompleted; // Show only pending tasks if active tab selected
        if (taskFilter === 'completed') return isCompleted; // Show only completed tasks if completed tab selected
        if (taskFilter === 'overdue') return isOverdue; // Show only overdue tasks if overdue tab selected
        return true; // Show all tasks by default
    }); // End filteredTasks array

    // Sort tasks based on selected sortBy dropdown control
    const sortedTasks = [...filteredTasks].sort((a, b) => {
        if (sortBy === 'priority') {
            return b.priority - a.priority; // Sort descending by priority (High -> Low)
        }
        if (sortBy === 'dueDate') {
            if (!a.dueDate) return 1; // Put tasks without due dates at the bottom
            if (!b.dueDate) return -1;
            return new Date(a.dueDate) - new Date(b.dueDate); // Sort ascending by due date (Earliest -> Latest)
        }
        return 0; // Default creation order
    }); // End sortedTasks array

    // --- Pagination Calculations ---
    const totalPages = Math.ceil(sortedTasks.length / tasksPerPage) || 1; // Calculate total number of pages needed
    const indexOfLastTask = currentPage * tasksPerPage; // Calculate index of last task on current page
    const indexOfFirstTask = indexOfLastTask - tasksPerPage; // Calculate index of first task on current page
    const currentTasks = sortedTasks.slice(indexOfFirstTask, indexOfLastTask); // Slice sorted tasks array for current page display

    // Handlers for pagination navigation buttons
    const handleNextPage = () => {
        if (currentPage < totalPages) setCurrentPage(currentPage + 1);
    };
    const handlePrevPage = () => {
        if (currentPage > 1) setCurrentPage(currentPage - 1);
    };

    const filteredProjects = projects.filter((project) => 
        project.name.toLowerCase().includes(projectSearch.toLowerCase())
    ); // End filteredProjects array

    const totalProjects = projects.length; // Calculate total number of projects
    const totalTasks = tasks.length; // Calculate total number of tasks in selected project
    const completedTasksCount = tasks.filter(t => t.isCompleted).length; // Calculate completed tasks count using boolean
    const activeTasksCount = totalTasks - completedTasksCount; // Calculate remaining active tasks count

    // --- Progress Bar Calculations ---
    const progressPercentage = totalTasks > 0 ? Math.round((completedTasksCount / totalTasks) * 100) : 0; // Calculate progress percentage safely

    const handleLogout = () => { // Function handler for logging out user
        localStorage.removeItem('jwt_token'); // Remove JWT token from browser local storage
        toast.success('Logged out successfully.'); // Trigger success toast
        navigate('/login'); // Redirect user to login route page
    }; // End handleLogout

    useEffect(() => { // React useEffect hook to load initial projects on component mount
        fetchProjects(); // Invoke fetchProjects function
    }, []); // Empty dependency array ensures execution only once on mount

    return ( // Return main component JSX markup
        <div className="min-h-screen bg-slate-900 text-slate-100 p-6 md:p-10">
            <div className="max-w-6xl mx-auto">
                {/* Header Navbar Section */}
                <div className="flex justify-between items-center bg-slate-800 border border-slate-700/60 p-5 rounded-xl shadow-md mb-6">
                    <div className="flex items-center gap-3">
                        <LayoutDashboard className="w-7 h-7 text-indigo-400" />
                        <h1 className="text-xl font-bold tracking-tight text-white">TaskFlow Pro Dashboard</h1>
                    </div>
                    
                    {/* Navigation Buttons: Profile & Log Out */}
                    <div className="flex items-center gap-3">
                        <button 
                            onClick={() => setIsProfileOpen(true)} 
                            className="flex items-center gap-2 bg-indigo-600/20 text-indigo-300 hover:bg-indigo-600/30 border border-indigo-500/30 px-3.5 py-2 rounded-lg text-sm font-medium transition-all"
                        >
                            <User className="w-4 h-4" /> Profile
                        </button>
                        <button 
                            onClick={handleLogout} 
                            className="flex items-center gap-2 bg-slate-700 hover:bg-red-600/20 hover:text-red-400 text-slate-300 px-4 py-2 rounded-lg text-sm font-medium transition-all"
                        >
                            <LogOut className="w-4 h-4" /> Log Out
                        </button>
                    </div>
                </div>

                {/* --- User Profile Modal --- */}
                {isProfileOpen && (
                    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
                        <div className="bg-slate-800 border border-slate-700 rounded-2xl w-full max-w-md p-6 shadow-2xl relative">
                            <div className="flex justify-between items-center mb-6">
                                <h3 className="text-lg font-bold text-white flex items-center gap-2">
                                    <Settings className="w-5 h-5 text-indigo-400" /> User Profile Settings
                                </h3>
                                <button onClick={() => setIsProfileOpen(false)} className="text-slate-400 hover:text-white p-1">
                                    <X className="w-5 h-5" />
                                </button>
                            </div>

                            <form onSubmit={handleSaveProfile} className="space-y-4">
                                <div>
                                    <label className="block text-xs font-medium text-slate-400 mb-1">Full Name</label>
                                    <input 
                                        type="text" 
                                        value={profileName} 
                                        onChange={(e) => setProfileName(e.target.value)} 
                                        className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-indigo-500 text-sm"
                                        required
                                    />
                                </div>
                                <div>
                                    <label className="block text-xs font-medium text-slate-400 mb-1">Email Address</label>
                                    <input 
                                        type="email" 
                                        value={profileEmail} 
                                        onChange={(e) => setProfileEmail(e.target.value)} 
                                        className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-indigo-500 text-sm"
                                        required
                                    />
                                </div>
                                <div className="flex justify-end gap-3 pt-4 border-t border-slate-700/60">
                                    <button 
                                        type="button" 
                                        onClick={() => setIsProfileOpen(false)} 
                                        className="px-4 py-2 bg-slate-700 hover:bg-slate-600 text-slate-200 text-xs rounded-lg font-medium"
                                    >
                                        Cancel
                                    </button>
                                    <button 
                                        type="submit" 
                                        className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white text-xs rounded-lg font-medium flex items-center gap-1.5"
                                    >
                                        <Save className="w-3.5 h-3.5" /> Save Changes
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                )}

                {/* Summary Metrics Cards Bar */}
                <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-8">
                    {/* Total Projects Card */}
                    <div className="bg-slate-800 border border-slate-700/60 p-5 rounded-xl shadow-sm flex items-center gap-4">
                        <div className="p-3 bg-indigo-500/10 border border-indigo-500/30 rounded-lg text-indigo-400">
                            <FolderPlus className="w-6 h-6" />
                        </div>
                        <div>
                            <p className="text-sm text-slate-400 font-medium">Total Projects</p>
                            <h4 className="text-2xl font-bold text-white mt-0.5">{totalProjects}</h4>
                        </div>
                    </div>

                    {/* Active Tasks Card */}
                    <div className="bg-slate-800 border border-slate-700/60 p-5 rounded-xl shadow-sm flex items-center gap-4">
                        <div className="p-3 bg-amber-500/10 border border-amber-500/30 rounded-lg text-amber-400">
                            <Clock className="w-6 h-6" />
                        </div>
                        <div>
                            <p className="text-sm text-slate-400 font-medium">Active Tasks</p>
                            <h4 className="text-2xl font-bold text-white mt-0.5">{selectedProject ? activeTasksCount : '-'}</h4>
                        </div>
                    </div>

                    {/* Completed Tasks Card */}
                    <div className="bg-slate-800 border border-slate-700/60 p-5 rounded-xl shadow-sm flex items-center gap-4">
                        <div className="p-3 bg-emerald-500/10 border border-emerald-500/30 rounded-lg text-emerald-400">
                            <CheckCircle2 className="w-6 h-6" />
                        </div>
                        <div>
                            <p className="text-sm text-slate-400 font-medium">Completed Tasks</p>
                            <h4 className="text-2xl font-bold text-white mt-0.5">{selectedProject ? completedTasksCount : '-'}</h4>
                        </div>
                    </div>
                </div>

                {/* Error Notification Banner */}
                {error && <div className="mb-6 p-4 bg-red-500/10 border border-red-500/30 text-red-400 rounded-lg">{error}</div>}

                {/* Main Two-Column Grid Layout */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                    {/* Left Column: Create Project & Project List */}
                    <div className="space-y-6">
                        <div className="bg-slate-800 border border-slate-700/60 p-6 rounded-xl shadow-md">
                            <h3 className="text-lg font-semibold text-white flex items-center gap-2 mb-4">
                                <FolderPlus className="w-5 h-5 text-indigo-400" /> Create Project
                            </h3>
                            <form onSubmit={handleCreateProject} className="space-y-4">
                                <input 
                                    type="text" 
                                    placeholder="Project Name" 
                                    value={projectName} 
                                    onChange={(e) => setProjectName(e.target.value)} 
                                    className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-indigo-500"
                                    required
                                />
                                <input 
                                    type="text" 
                                    placeholder="Project Description" 
                                    value={projectDesc} 
                                    onChange={(e) => setProjectDesc(e.target.value)} 
                                    className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-indigo-500"
                                    required
                                />
                                <button type="submit" className="w-full bg-indigo-600 hover:bg-indigo-500 text-white font-medium py-2 rounded-lg transition-colors">
                                    Add Project
                                </button>
                            </form>
                        </div>

                        <div>
                            <div className="flex justify-between items-center mb-3">
                                <h3 className="text-lg font-semibold text-white">Your Projects</h3>
                            </div>

                            <div className="relative mb-4">
                                <span className="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none text-slate-400">
                                    <Search className="w-4 h-4" />
                                </span>
                                <input 
                                    type="text"
                                    placeholder="Search projects by title..."
                                    value={projectSearch}
                                    onChange={(e) => setProjectSearch(e.target.value)}
                                    className="w-full bg-slate-800 border border-slate-700/60 rounded-xl pl-10 pr-4 py-2.5 text-white text-sm focus:outline-none focus:border-indigo-500 shadow-sm"
                                />
                            </div>

                            {loadingProjects ? (
                                <div className="flex flex-col items-center justify-center p-12 bg-slate-800/40 border border-slate-700/40 rounded-xl">
                                    <Loader2 className="w-8 h-8 text-indigo-500 animate-spin mb-2" />
                                    <p className="text-slate-400 text-sm">Loading projects from server...</p>
                                </div>
                            ) : (
                                <div className="space-y-3">
                                    {filteredProjects.length === 0 ? (
                                        <p className="text-slate-400 text-sm italic">No projects found matching your search.</p>
                                    ) : (
                                        filteredProjects.map((project) => {
                                            const isEditing = editingProjectId === project.id;
                                            return (
                                                <div 
                                                    key={project.id} 
                                                    onClick={() => !isEditing && handleSelectProject(project)} 
                                                    className={'p-4 rounded-xl border transition-all ' + (selectedProject && selectedProject.id === project.id ? 'bg-indigo-950/40 border-indigo-500 shadow-md shadow-indigo-500/10' : 'bg-slate-800 border-slate-700/60 hover:border-slate-600') + (!isEditing ? ' cursor-pointer' : '')}
                                                >
                                                    {isEditing ? (
                                                        <div className="space-y-3" onClick={(e) => e.stopPropagation()}>
                                                            <input 
                                                                type="text" 
                                                                value={editProjectName} 
                                                                onChange={(e) => setEditProjectName(e.target.value)} 
                                                                className="w-full bg-slate-900 border border-indigo-500 rounded-lg px-3 py-1.5 text-white text-sm focus:outline-none"
                                                                required
                                                            />
                                                            <input 
                                                                type="text" 
                                                                value={editProjectDesc} 
                                                                onChange={(e) => setEditProjectDesc(e.target.value)} 
                                                                className="w-full bg-slate-900 border border-indigo-500 rounded-lg px-3 py-1.5 text-white text-sm focus:outline-none"
                                                                required
                                                            />
                                                            <div className="flex justify-end gap-2 pt-1">
                                                                <button onClick={handleCancelEditProject} className="px-3 py-1 bg-slate-700 hover:bg-slate-600 text-slate-200 text-xs rounded-lg font-medium flex items-center gap-1">
                                                                    <X className="w-3.5 h-3.5" /> Cancel
                                                                </button>
                                                                <button onClick={(e) => handleUpdateProject(project.id, e)} className="px-3 py-1 bg-indigo-600 hover:bg-indigo-500 text-white text-xs rounded-lg font-medium flex items-center gap-1">
                                                                    <Check className="w-3.5 h-3.5" /> Save
                                                                </button>
                                                            </div>
                                                        </div>
                                                    ) : (
                                                        <div className="flex items-start justify-between gap-3">
                                                            <div>
                                                                <h4 className="font-semibold text-white">{project.name}</h4>
                                                                <p className="text-sm text-slate-400 mt-1">{project.description}</p>
                                                            </div>
                                                            <div className="flex items-center gap-1">
                                                                <button onClick={(e) => handleStartEditProject(project, e)} className="text-slate-400 hover:text-indigo-400 p-1.5 transition-colors" title="Edit project">
                                                                    <Edit3 className="w-4 h-4" />
                                                                </button>
                                                                <button onClick={(e) => handleDeleteProject(project.id, e)} className="text-slate-400 hover:text-red-400 p-1.5 transition-colors" title="Delete project">
                                                                    <Trash2 className="w-4 h-4" />
                                                                </button>
                                                            </div>
                                                        </div>
                                                    )}
                                                </div>
                                            );
                                        })
                                    )}
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Right Column: Task Management & Pagination Controls */}
                    <div>
                        {selectedProject ? (
                            <div className="space-y-6">
                                <div className="bg-slate-800 border border-slate-700/60 p-6 rounded-xl shadow-md">
                                    <h3 className="text-lg font-semibold text-white flex items-center gap-2 mb-4">
                                        <PlusCircle className="w-5 h-5 text-emerald-400" /> Add Task to {selectedProject.name}
                                    </h3>
                                    <form onSubmit={handleCreateTask} className="space-y-4">
                                        <input 
                                            type="text" 
                                            placeholder="Task Title" 
                                            value={taskTitle} 
                                            onChange={(e) => setTaskTitle(e.target.value)} 
                                            className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-emerald-500"
                                            required
                                        />
                                        <input 
                                            type="text" 
                                            placeholder="Task Description" 
                                            value={taskDesc} 
                                            onChange={(e) => setTaskDesc(e.target.value)} 
                                            className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-emerald-500"
                                            required
                                        />
                                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                            <div>
                                                <label className="block text-xs font-medium text-slate-400 mb-1">Priority</label>
                                                <select 
                                                    value={taskPriority} 
                                                    onChange={(e) => setTaskPriority(e.target.value)} 
                                                    className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-emerald-500 text-sm"
                                                >
                                                    <option value={0}>Low</option>
                                                    <option value={1}>Medium</option>
                                                    <option value={2}>High</option>
                                                </select>
                                            </div>
                                            <div>
                                                <label className="block text-xs font-medium text-slate-400 mb-1">Due Date</label>
                                                <input 
                                                    type="date"
                                                    value={taskDueDate}
                                                    onChange={(e) => setTaskDueDate(e.target.value)}
                                                    className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2 text-white focus:outline-none focus:border-emerald-500 text-sm [color-scheme:dark]"
                                                />
                                            </div>
                                        </div>
                                        <button type="submit" className="w-full bg-emerald-600 hover:bg-emerald-500 text-white font-medium py-2 rounded-lg transition-colors">
                                            Add Task
                                        </button>
                                    </form>
                                </div>

                                <div>
                                    {/* Project Progress Bar Section */}
                                    <div className="bg-slate-800 border border-slate-700/60 p-4 rounded-xl mb-4 shadow-sm">
                                        <div className="flex justify-between items-center mb-2 text-xs font-medium text-slate-300">
                                            <span>Project Completion Progress</span>
                                            <span className="text-indigo-400 font-bold">{progressPercentage}%</span>
                                        </div>
                                        <div className="w-full bg-slate-900 rounded-full h-2.5 overflow-hidden border border-slate-700/50">
                                            <div 
                                                className="bg-gradient-to-r from-indigo-500 to-emerald-500 h-2.5 rounded-full transition-all duration-500" 
                                                style={{ width: `${progressPercentage}%` }}
                                            ></div>
                                        </div>
                                    </div>

                                    <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3 mb-3">
                                        <h3 className="text-lg font-semibold text-white">Tasks</h3>
                                        
                                        <div className="flex items-center gap-2 w-full sm:w-auto flex-wrap">
                                            <div className="flex items-center gap-1.5 bg-slate-800 border border-slate-700/60 rounded-lg px-2.5 py-1 text-xs text-slate-300">
                                                <ArrowUpDown className="w-3.5 h-3.5 text-indigo-400" />
                                                <select 
                                                    value={sortBy} 
                                                    onChange={(e) => setSortBy(e.target.value)} 
                                                    className="bg-transparent text-slate-200 focus:outline-none text-xs cursor-pointer"
                                                >
                                                    <option value="created" className="bg-slate-800">Default</option>
                                                    <option value="priority" className="bg-slate-800">Highest Priority</option>
                                                    <option value="dueDate" className="bg-slate-800">Earliest Due Date</option>
                                                </select>
                                            </div>

                                            {/* Task Filter Tabs including Overdue */}
                                            <div className="flex bg-slate-800 border border-slate-700/60 p-1 rounded-lg text-xs font-medium">
                                                <button onClick={() => setTaskFilter('all')} className={'px-3 py-1 rounded-md transition-all ' + (taskFilter === 'all' ? 'bg-indigo-600 text-white shadow-sm' : 'text-slate-400 hover:text-white')}>All</button>
                                                <button onClick={() => setTaskFilter('active')} className={'px-3 py-1 rounded-md transition-all ' + (taskFilter === 'active' ? 'bg-indigo-600 text-white shadow-sm' : 'text-slate-400 hover:text-white')}>Active</button>
                                                <button onClick={() => setTaskFilter('completed')} className={'px-3 py-1 rounded-md transition-all ' + (taskFilter === 'completed' ? 'bg-indigo-600 text-white shadow-sm' : 'text-slate-400 hover:text-white')}>Completed</button>
                                                <button onClick={() => setTaskFilter('overdue')} className={'px-3 py-1 rounded-md transition-all ' + (taskFilter === 'overdue' ? 'bg-red-600 text-white shadow-sm' : 'text-slate-400 hover:text-white')}>Overdue</button>
                                            </div>
                                        </div>
                                    </div>

                                    <div className="relative mb-4">
                                        <span className="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none text-slate-400">
                                            <Search className="w-4 h-4" />
                                        </span>
                                        <input 
                                            type="text"
                                            placeholder="Search tasks in this project..."
                                            value={taskSearch}
                                            onChange={(e) => { setTaskSearch(e.target.value); setCurrentPage(1); }}
                                            className="w-full bg-slate-800 border border-slate-700/60 rounded-xl pl-10 pr-4 py-2.5 text-white text-sm focus:outline-none focus:border-emerald-500 shadow-sm"
                                        />
                                    </div>

                                    {loadingTasks ? (
                                        <div className="flex flex-col items-center justify-center p-12 bg-slate-800/40 border border-slate-700/40 rounded-xl">
                                            <Loader2 className="w-8 h-8 text-emerald-500 animate-spin mb-2" />
                                            <p className="text-slate-400 text-sm">Loading tasks...</p>
                                        </div>
                                    ) : (
                                        <div>
                                            <div className="space-y-3">
                                                {currentTasks.length === 0 ? (
                                                    <p className="text-slate-400 text-sm italic">No tasks found matching your filter.</p>
                                                ) : (
                                                    currentTasks.map((task) => {
                                                        const isCompleted = task.isCompleted;
                                                        const isEditing = editingTaskId === task.id;
                                                        const isOverdue = task.dueDate && !isCompleted && new Date(task.dueDate) < new Date(new Date().toDateString());
                                                        const formattedDate = task.dueDate ? new Date(task.dueDate).toLocaleDateString() : null;

                                                        return (
                                                            <div 
                                                                key={task.id} 
                                                                className={'bg-slate-800 p-4 rounded-xl shadow-sm border transition-all ' + (isOverdue ? 'border-red-500/80 bg-red-950/10' : 'border-slate-700/60')}
                                                            >
                                                                {isEditing ? (
                                                                    <div className="space-y-3">
                                                                        <input 
                                                                            type="text"
                                                                            value={editTaskTitle}
                                                                            onChange={(e) => setEditTaskTitle(e.target.value)}
                                                                            className="w-full bg-slate-900 border border-indigo-500 rounded-lg px-3 py-1.5 text-white text-sm focus:outline-none"
                                                                            required
                                                                        />
                                                                        <input 
                                                                            type="text"
                                                                            value={editTaskDesc}
                                                                            onChange={(e) => setEditTaskDesc(e.target.value)}
                                                                            className="w-full bg-slate-900 border border-indigo-500 rounded-lg px-3 py-1.5 text-white text-sm focus:outline-none"
                                                                        />
                                                                        <div className="grid grid-cols-2 gap-2">
                                                                            <select 
                                                                                value={editTaskPriority}
                                                                                onChange={(e) => setEditTaskPriority(e.target.value)}
                                                                                className="bg-slate-900 border border-slate-700 rounded-lg px-2 py-1 text-white text-xs focus:outline-none"
                                                                            >
                                                                                <option value={0}>Low</option>
                                                                                <option value={1}>Medium</option>
                                                                                <option value={2}>High</option>
                                                                            </select>
                                                                            <input 
                                                                                type="date"
                                                                                value={editTaskDueDate}
                                                                                onChange={(e) => setEditTaskDueDate(e.target.value)}
                                                                                className="bg-slate-900 border border-slate-700 rounded-lg px-2 py-1 text-white text-xs focus:outline-none [color-scheme:dark]"
                                                                            />
                                                                        </div>
                                                                        <div className="flex justify-end gap-2 pt-1">
                                                                            <button onClick={handleCancelEditTask} className="px-3 py-1 bg-slate-700 hover:bg-slate-600 text-slate-200 text-xs rounded-lg font-medium flex items-center gap-1">
                                                                                <X className="w-3.5 h-3.5" /> Cancel
                                                                            </button>
                                                                            <button onClick={() => handleUpdateTask(task.id, task)} className="px-3 py-1 bg-indigo-600 hover:bg-indigo-500 text-white text-xs rounded-lg font-medium flex items-center gap-1">
                                                                                <Check className="w-3.5 h-3.5" /> Save
                                                                            </button>
                                                                        </div>
                                                                    </div>
                                                                ) : (
                                                                    <div className="flex items-start justify-between gap-4">
                                                                        <div className="flex items-start gap-3">
                                                                            <button onClick={() => handleToggleTask(task)} className="mt-1 text-slate-400 hover:text-emerald-400 transition-colors">
                                                                                {isCompleted ? <CheckSquare className="w-5 h-5 text-emerald-400" /> : <Square className="w-5 h-5" />}
                                                                            </button>
                                                                            <div>
                                                                                <div className="flex items-center gap-2">
                                                                                    <h4 className={'font-medium text-white ' + (isCompleted ? 'line-through text-slate-500' : '')}>{task.title}</h4>
                                                                                    {isOverdue && (
                                                                                        <span className="flex items-center gap-1 px-2 py-0.5 text-[10px] font-bold bg-red-500/20 text-red-400 border border-red-500/40 rounded-full animate-pulse">
                                                                                            <AlertCircle className="w-3 h-3" /> Overdue
                                                                                        </span>
                                                                                    )}
                                                                                </div>
                                                                                <p className={'text-sm mt-1 ' + (isCompleted ? 'text-slate-600 line-through' : 'text-slate-400')}>{task.description}</p>
                                                                                
                                                                                {formattedDate && (
                                                                                    <div className={'flex items-center gap-1 mt-2 text-xs ' + (isOverdue ? 'text-red-400 font-medium' : 'text-slate-400')}>
                                                                                        <Calendar className={'w-3.5 h-3.5 ' + (isOverdue ? 'text-red-400' : 'text-indigo-400')} />
                                                                                        <span>Due: {formattedDate}</span>
                                                                                    </div>
                                                                                )}
                                                                            </div>
                                                                        </div>
                                                                        <div className="flex items-center gap-3">
                                                                            {renderPriorityBadge(task.priority)}
                                                                            <button onClick={() => handleStartEditTask(task)} className="text-slate-400 hover:text-indigo-400 transition-colors p-1" title="Edit task">
                                                                                <Edit3 className="w-4 h-4" />
                                                                            </button>
                                                                            <button onClick={() => handleDeleteTask(task.id)} className="text-slate-500 hover:text-red-400 transition-colors p-1" title="Delete task">
                                                                                <Trash2 className="w-4 h-4" />
                                                                            </button>
                                                                        </div>
                                                                    </div>
                                                                )}
                                                            </div>
                                                        );
                                                    })
                                                )}
                                            </div>

                                            {/* Pagination Control Bar */}
                                            {sortedTasks.length > tasksPerPage && (
                                                <div className="flex justify-between items-center mt-4 pt-3 border-t border-slate-800 text-xs text-slate-400">
                                                    <span>Showing {indexOfFirstTask + 1} to {Math.min(indexOfLastTask, sortedTasks.length)} of {sortedTasks.length} tasks</span>
                                                    <div className="flex items-center gap-2">
                                                        <button 
                                                            onClick={handlePrevPage} 
                                                            disabled={currentPage === 1}
                                                            className="flex items-center gap-1 px-3 py-1.5 bg-slate-800 border border-slate-700/60 rounded-lg text-slate-200 hover:bg-slate-700 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
                                                        >
                                                            <ChevronLeft className="w-4 h-4" /> Prev
                                                        </button>
                                                        <span className="font-medium text-slate-200">Page {currentPage} of {totalPages}</span>
                                                        <button 
                                                            onClick={handleNextPage} 
                                                            disabled={currentPage === totalPages}
                                                            className="flex items-center gap-1 px-3 py-1.5 bg-slate-800 border border-slate-700/60 rounded-lg text-slate-200 hover:bg-slate-700 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
                                                        >
                                                            Next <ChevronRight className="w-4 h-4" />
                                                        </button>
                                                    </div>
                                                </div>
                                            )}
                                        </div>
                                    )}
                                </div>
                            </div>
                        ) : (
                            <div className="h-full flex flex-col items-center justify-center p-12 bg-slate-800/40 border border-slate-700/40 rounded-xl text-center">
                                <LayoutDashboard className="w-12 h-12 text-slate-600 mb-3" />
                                <p className="text-slate-400">Select a project from the left to view and manage its tasks.</p>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}