import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GradeViewComponent } from './grade-view/grade-view.component';

const routes: Routes = [{ path: '', component: GradeViewComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GradesRoutingModule { }
