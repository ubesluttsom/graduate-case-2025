import { Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import Start from "./pages/Start";
import ExcursionOverview from "./pages/ExcursionOverview";

const App = () => {

  return (
    <Routes>
      <Route path="/" element={<Start />}>
      </Route>
      <Route path ="ExcursionOverview" element={<ExcursionOverview/>}>
      </Route>
    </Routes>
  )

}

export default App;
