import { Route, Routes } from "react-router-dom";
import Home from "./pages/Home";
import ExcursionOverview from "./pages/ExcursionOverview";
import WhaleSafari from "./pages/Whale-safari";

const App = () => {

  return (
    <Routes>
      <Route path="/" element={<Home />}></Route>
      <Route path ="ExcursionOverview" element={<ExcursionOverview/>}>
      </Route>      
      <Route path="/whaleSafari" element={<WhaleSafari />}>
      </Route>
    </Routes>
  )

}

export default App;
